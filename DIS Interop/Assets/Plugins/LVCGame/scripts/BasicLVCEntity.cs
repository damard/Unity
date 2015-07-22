/*
 *   Copyright 2013 Calytrix Technologies
 *
 *   This file is part of LVC Game for Unity.
 *
 *   NOTICE:  All information contained herein is, and remains
 *            the property of Calytrix Technologies Pty Ltd.
 *            The intellectual and technical concepts contained
 *            herein are proprietary to Calytrix Technologies Pty Ltd.
 *            Dissemination of this information or reproduction of
 *            this material is strictly forbidden unless prior written
 *            permission is obtained from Calytrix Technologies Pty Ltd.
 *
 *   Unless required by applicable law or agreed to in writing,
 *   software distributed under the License is distributed on an
 *   "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *   KIND, either express or implied.  See the License for the
 *   specific language governing permissions and limitations
 *   under the License.
 */

using System;
using UnityEngine;
using System.Collections;
using LVCGame;

/**
 * This is a MonoBehaviour which monitors the position and orientation of 
 * a Unity GameObject and sends out "automated" LVC updates containing this
 * information.
 * 
 * Compare this with ExternalLVCEntityHelper which handles *incoming* 
 * updates related to entities controlled by an external simulation system.
 * 
 * This allows more or less any Unity GameObject to become part of an LVC
 * Game simply by dragging this behaviour onto the GameObject in the Heirarchy
 * view of the Unity Editor.
 * 
 * Currently the sent updates actually populate the following EntityUpdate 
 * properties with values calculated from the game object:
 *    - entityId
 *    - position
 *    - orientation
 *    - worldVelocity
 *    - worldAcceleration
 *    - bodyAngularVelocity
 * 
 * The following values must be manually set as script parameters
 *    - entityGameType  (default: "UNKNOWN TYPE")
 *    - entityLvcType   (default: "UNKNOWN TYPE")
 *    - marking         (default: "UNMARKED")
 * 
 * The following values are *not* populated/modified in the updates, since
 * these are very specific to the GameObject in question and so cannot be
 * handled in a generic way:
 *    - userData
 *    - parentEntityId
 *    - force
 *    - stance
 *    - primaryWeaponPosture
 *    - secondaryWeaponPosture
 *    - damage
 *    - appearance
 *    - lights
 *    - boundingBox
 *    - anything related to articulations
 *    - anything related to fires
 * 
 * These automated updates are triggered by any one of the following conditions:
 *    - A certain time interval has passed. By default this is 1 second.
 *    - The game object has moved more than a certain distance from its last 
 *      reported position. By default this is 1.0 units.
 *    - The game object has rotated more than a certain angle from its last 
 *      reported position. By default this is 1.0 degrees.
 */
public class BasicLVCEntity : MonoBehaviour 
{
	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	protected static readonly string UNKNOWN_GAME_TYPE = "UNKNOWN GAME TYPE";
	protected static readonly string UNMARKED = "UNMARKED";
	
	private static readonly Color DAMAGE_000 = new Color(1,1,1,1); // 0% damage - white
	private static readonly Color DAMAGE_100 = new Color(0.1F,0.1F,0.1F,1); // 100% damage - charcoal grey
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------
	// ---- PUBLIC - Note that these will also be available in 
	//               the Unity GUI for configuration.
	public string gameType = UNKNOWN_GAME_TYPE;
	public string marking = UNMARKED;
	public LVCGame.UnitForceType force = LVCGame.UnitForceType.Neutral;
	
	public bool showLogging = false;

	// the minimum amount the game object must move before an update is sent 
	public float updateDistanceThreshold = 1.0f;
	// the minimum amount the game object must rotate before an update is sent 
	public float updateRotationThreshold = 1.0f;
	// regardless of how much the game object has moved/rotated, we update at
	// at least this interval in seconds (i.e., even if the object remains 
	// completely stationary, unmoving and not ratating, LVC updates will be 
	// sent this often)
	public float maxUpdateWait = 1.0f;
	
	// ---- PRIVATE - only available internally ---------------
	// these values are used to calculate various statistics on the movement
	// of the game object, such as acceleration, angular velocity and so on
	protected UnityEngine.Vector3 lastUpdatedPosition;
	protected Quaternion lastUpdatedRotation;
	protected UnityEngine.Vector3 lastVelocity;
	protected float updateDistanceSquared;
	protected float lastUpdatedTime = 0.0f;

	// this is the LVC EntityDataManager via which we send our updates
	protected EntityDataManager entityDataManager;
	
	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	
	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	
	////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////// MonoBehaviour Overrides //////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * OVERRIDE
	 * 
	 * Used for initialization. Awake is *always* called, and the call happens before Start()
	 */
	void Awake () 
	{
		InitialiseEntityDataManager();
	}
	
	/**
	 * OVERRIDE
	 * 
	 * Used for initialization. Start is only called if the script is enabled.
	 */
	void Start () 
	{
		// distance magintudes are faster to calculate than distances because
		// they avoid the costly square root calculation - initialise it using 
		// the distance threshold value which was set
		updateDistanceSquared = ( updateDistanceThreshold * updateDistanceThreshold );
		
		// initialise entity bounding box
		// Bounds bounds = gameObject.GetComponent<MeshFilter>().mesh.bounds;
		// TransferValues((bounds.max - bounds.min), ref entityData.properties.boundingBox);

		// initialise entity location/rotation details
		LVCUtils.TransferValues( transform.position, ref lastUpdatedPosition );
		entityDataManager.SetPosition( transform.position );
		LVCUtils.TransferValues( transform.rotation, ref lastUpdatedRotation );
		entityDataManager.SetOrientation( transform.rotation );
		
		// register this entity with the ambassador
		entityDataManager.Create( gameObject );
		
		if( showLogging )
			Debug.Log(Time.time.ToString("f5") + " [CREATE] : "+entityDataManager.DisplayFormatted() );
	}
	
	/**
	 * OVERRIDE
	 * 
	 * Update is called once per frame
	 */
	void Update () 
	{
		// inform LVC of position, rotation etc
		DoLVCUpdate();
		
		// change colour of entity to indicate damage level ------------------------
		float damage = entityDataManager.GetDamage();
		if( damage > 0.0F )
		{
			Color damageColor = Color.Lerp( DAMAGE_000, DAMAGE_100, damage );
			Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
			foreach( Renderer renderer in renderers )
				foreach( Material material in renderer.materials )
					material.color = damageColor;
		}	
		
		// check for any entity type specific handling for updates
		LVCExtraUpdateHandler extraUpdateHandler = gameObject.GetComponent<LVCExtraUpdateHandler>();
		if( extraUpdateHandler!=null )
			extraUpdateHandler.HandleExtra( entityDataManager, gameObject );		
	}
	
	/**
	 * OVERRIDE
	 * 
	 * OnDisable is called when the object is destroyed and can be used for any cleanup code.
	 */
	void OnDisable()
	{
		entityDataManager.Delete();
		
		if( showLogging )
			Debug.Log(Time.time.ToString("f5") + " [DELETE] : "+entityDataManager.DisplayFormatted() );
	} 
	
	////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// LVC Game Related Methods //////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	public void FireWeapon( string lvcMunitionType, UnityEngine.Vector3 origin, UnityEngine.Vector3 velocity,
							uint quantity, uint rate, 
						    LVCGame.WarHeadType warhead, 
							LVCGame.FuseType fuse,
							long targetId
						  )
	{
		entityDataManager.FireWeapon( lvcMunitionType, origin, velocity,
									  quantity, rate, 
						    		  warhead, fuse,
									  targetId 
									);
	}
	
	/**
	 * Cause a basic LVC game update to occur for this game object (position, rotation), based on
	 * the settings for movement, rotation and time thresholds
	 */ 
	protected void DoLVCUpdate()
	{
		DoLVCUpdate( false );
	}
	
	/**
	 * Cause a basic LVC game update to occur for this game object (position, rotation), based on
	 * the settings for movement, rotation and time thresholds
	 * @param forced if true, an update will be sent. If false, an update will be sent if the
	 *        conditions of the configured movement, rotation and time thresholds are met.
	 */ 
	protected void DoLVCUpdate( bool forced )
	{
		float now = Time.time;
		
		// time
		float timeElapsed = (now - lastUpdatedTime);
		// distance
		UnityEngine.Vector3 displacement = ( transform.position - lastUpdatedPosition );
		float squareOfDistance = displacement.sqrMagnitude;
		// rotation
		float rotationAngle = Quaternion.Angle( transform.rotation, lastUpdatedRotation );
		
		// first determine if we actually need to send an update at all:
		// if we're forcing, the time threshold has been exceeded, the displacement threshold
		// has been exceeded or the rotation threshold has been exceeded, we send an update
		bool doUpdate = forced || 
				        ( timeElapsed >= maxUpdateWait ) ||
					    ( squareOfDistance >= updateDistanceSquared ) || 
					    ( rotationAngle >= updateRotationThreshold );
		
		if( doUpdate )
		{
			// there is sufficient reason to send out an update, so do that now
			
			// update location and orientation
			entityDataManager.SetPosition( transform.position );
			entityDataManager.SetOrientation( transform.rotation );
			// update the world and angular velocities
			UnityEngine.Vector3 worldVelocity = CalculateWorldVelocity( displacement, timeElapsed );
			entityDataManager.SetWorldVelocity( worldVelocity ); 
			UnityEngine.Vector3 angularVelocity = CalculateAngularVelocity( lastUpdatedRotation, transform.rotation, timeElapsed ); 
			entityDataManager.SetBodyAngularVelocity( angularVelocity ); 
			// update the acceleration
			UnityEngine.Vector3 acceleration = CalculateAcceleration( lastVelocity, worldVelocity, timeElapsed );
			entityDataManager.SetWorldAcceleration( acceleration ); 
			
			// send the update
			entityDataManager.Update();
			if( showLogging )
				Debug.Log( Time.time.ToString("f5") + " [UPDATE] "+timeElapsed.ToString("f3")+": "+entityDataManager.DisplayFormatted() );
			
			// cache the current velocity so that we can calculate the acceleration next time around
			LVCUtils.TransferValues( worldVelocity, ref lastVelocity );
			// cache the current position and rotation so we can detect thresholds next time
			LVCUtils.TransferValues( transform.position, ref lastUpdatedPosition );
			LVCUtils.TransferValues( transform.rotation, ref lastUpdatedRotation );
			// cache the time sent so we can time our next update appropriately
			lastUpdatedTime = now;
		}
	}
	
	private void InitialiseEntityDataManager()
	{
		if( this.entityDataManager == null )
			this.entityDataManager = new EntityDataManager( this.gameType, this.marking, this.force );
	}
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Utility/Convenience Methods ////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * A utility function to calculate the world velocity of the game object and store it in an LVC Vector3
	 * 
	 * @param displacement the UnityEngine.Vector3 instance which is the source of the displacement values
	 * @param seconds the duration over which the displacement occurred
	 * @param lvcV3 the LVCGame.Vector3 instance which is to receive the velocity values
	 */
	protected UnityEngine.Vector3 CalculateWorldVelocity( UnityEngine.Vector3 displacement, float seconds )
	{
		UnityEngine.Vector3 result = UnityEngine.Vector3.zero;
		result.x = displacement.x / seconds;
		result.y = displacement.y / seconds;
		result.z = displacement.z / seconds;
		return result;
	}

	/**
	 * A utility function to calculate the acceleration of the game object and store it in an LVC Vector3
	 * 
	 * @param start the LVCGame.Vector3 instance which is the source of the initial velocity
	 * @param end the LVCGame.Vector3 instance which is the source of the final velocity
	 * @param seconds the duration over which the change in velocity occurred
	 * @param acceleration the LVCGame.Vector3 instance which is to receive the acceleration values
	 */
	protected UnityEngine.Vector3 CalculateAcceleration( UnityEngine.Vector3 start, UnityEngine.Vector3 end, float seconds )
	{
		UnityEngine.Vector3 result = UnityEngine.Vector3.zero;
		result.x = (end.x - start.x) / seconds;
		result.y = (end.y - start.y) / seconds;
		result.z = (end.z - start.z) / seconds;
		return result;
	}
	
	/**
	 * A utility function to calculate the angular velocity of the game object and store it in an LVC Vector3
	 * 
	 * @param angle the Quaternion instance which is the source of the angular displacement values
	 * @param seconds the duration over which the rotation occurred
	 * @param lvcV3 the LVCGame.Vector3 instance which is to receive the angular velocity values
	 */
	protected UnityEngine.Vector3 CalculateAngularVelocity( Quaternion start, Quaternion end, float seconds )
	{
		UnityEngine.Vector3 result = UnityEngine.Vector3.zero;
		result.x = (end.eulerAngles.x - start.eulerAngles.x) / seconds;
		result.y = (end.eulerAngles.y - start.eulerAngles.y) / seconds;
		result.z = (end.eulerAngles.z - start.eulerAngles.z) / seconds;
		return result;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * Obtain the LVC EntityDataManager
	 * 
	 * @return LVC EntityDataManager
	 */
	public EntityDataManager GetEntityDataManager()
	{
		if( this.entityDataManager == null )
			InitialiseEntityDataManager();
		
		return this.entityDataManager;
	}
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
}
