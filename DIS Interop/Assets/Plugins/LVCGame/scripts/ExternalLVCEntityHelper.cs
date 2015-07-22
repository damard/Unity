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
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using LVCGame;

/**
 * This is a MonoBehaviour which provides basic "automated" updates based on 
 * information contained in *incoming* LVC traffic. The behaviour tries to...
 *  - find (or create) the Unity representation for the externally 'owned' entity
 *  - update the position, rotation and appearance of the Unity representation
 *
 * Compare this with BasicLVCEntity which handles *outgoing* updates related
 * to entities controlled by Unity.
 * 
 * These automated updates are triggered by the Update() method, which checks
 * for pending updates in the LVC queue which need processing.
 * 
 * Like the AssetManager, this behaviour should be attached to 
 * a *single* Unity GameObject acting as an "LVC Helper". This GameObject 
 * is invisible in the game, and does not directly take part in gameplay.
 * 
 * Usually this is done as follows:
 *  - go to the GameObject menu in the Unity editor
 *  - select "Create Empty"
 *  - rename the created GameObject "_LVCHelper"
 *  - drag this behaviour on top of that empty object
 *
 * You may also wish to position this helper object at some "extreme" 
 * coordinates, such as (1000, 1000, 1000), to keep it out of the way and 
 * avoid cluttering up the scene editor view, but this is not necessary.
 * 
 * See also: AssetManager
 */
public class ExternalLVCEntityHelper : MonoBehaviour 
{
	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	private static readonly Color DAMAGE_000 = new Color(1,1,1,1); // 0% damage - white
	private static readonly Color DAMAGE_100 = new Color(0.1F,0.1F,0.1F,1); // 100% damage - charcoal grey
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------
	// ---- PUBLIC - Note that these will also be available in 
	//               the Unity GUI for configuration.
	public bool showLogging = false;
	public GameObject defaultPrefab; // this is used if no prefab can be found in the asset manager
	public GameObject miniMunitionDetonation; // this is instantiated for miniature munitions
	public string miniMunitionTypes=""; // comma separated list of LVC names of miniature munitions
	public GameObject smallMunitionDetonation; // this is instantiated for small munitions
	public string smallMunitionTypes=""; // comma separated list of LVC names of small munitions
	public GameObject mediumMunitionDetonation; // this is instantiated for medium munitions
	public string mediumMunitionTypes=""; // comma separated list of LVC names of medium munitions
	public GameObject largeMunitionDetonation; // this is instantiated for large munitions
	public string largeMunitionTypes=""; // comma separated list of LVC names of large munitions
	public GameObject extraLargeMunitionDetonation;  // this is instantiated for extra large munitions
	public string extraLargeMunitionTypes=""; // comma separated list of LVC names of extra large munitions
	public GameObject smokeMunitionPrefab; // this is instantiated for smoke munitions
	public string smokeMunitionTypes=""; // comma separated list of LVC names/colours of smoke munitions
	
	// ---- PRIVATE - only available internally ---------------
	private Dictionary<string, GameObject> munitionDetonationMap;
	private Dictionary<string, Color>  smokeMunitionColorMap;
	// this is the LVC client via which we send our updates
	private LVCUnityAmbassador lvcUnityAmbassador;
	
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
	void OnEnable()
	{
		// initialise the LVC ambassador instance we will use for updates
		this.lvcUnityAmbassador = LVCUnityAmbassador.GetInstance();
		
		// create a lookup table for munition type names vs detonation effects
		List<LVCPair<string, GameObject>> munitionDetonations = new List<LVCPair<string, GameObject>>();
		munitionDetonations.Add( new LVCPair<string, GameObject>(miniMunitionTypes.Trim(), miniMunitionDetonation) );
		munitionDetonations.Add( new LVCPair<string, GameObject>(smallMunitionTypes.Trim(), smallMunitionDetonation) );
		munitionDetonations.Add( new LVCPair<string, GameObject>(mediumMunitionTypes.Trim(), mediumMunitionDetonation) );
		munitionDetonations.Add( new LVCPair<string, GameObject>(largeMunitionTypes.Trim(), largeMunitionDetonation) );
		munitionDetonations.Add( new LVCPair<string, GameObject>(extraLargeMunitionTypes.Trim(), extraLargeMunitionDetonation) );
		munitionDetonationMap = new Dictionary<string, GameObject>();
		// go through each of the detonation sizes
		foreach(LVCPair<string, GameObject> pair in munitionDetonations)
		{
			// if there is no detonation effect for this size, skip this part
			if( pair.B == null)
				continue;
			
			// create a lookup for the munition name to the detonation effect
			string[] munitionTypeNames = pair.A.Split(',');
			foreach( string munitionTypeName in munitionTypeNames )
			{
				munitionDetonationMap.Add( munitionTypeName.Trim(), pair.B );
			}
		}
		
		// create a lookup table for smoke munition type names. The smoke munition
		// names are expected to be in the form:
		//    LVCNAME-COLOUR
		// ...for example:
		//    smokeBomb-FF00FF
		// which would result in a magenta smoke flare being instantiated.
		smokeMunitionColorMap = new Dictionary<string, Color>();
		string[] smokeMunitionTypeNames = smokeMunitionTypes.Split(',');
		foreach( string munitionTypeName in smokeMunitionTypeNames )
		{
			string[] nameAndColour = munitionTypeName.Trim().Split('-');
			if( nameAndColour.Length == 2)
			{
				Color smokeColor = LVCUtils.hexToColor( nameAndColour[1] );
				smokeMunitionColorMap.Add( nameAndColour[0], smokeColor );
			}
		}		
	}
	
	/**
	 * OVERRIDE
	 * 
	 */
	void FixedUpdate () 
	{
		// get the pending remote LVC events which need to be processed
		EntityData[] pendingCreates = this.lvcUnityAmbassador.GetPendingRemoteCreates();
		EntityData[] pendingUpdates = this.lvcUnityAmbassador.GetPendingRemoteUpdates();
		long[] pendingDeletes = this.lvcUnityAmbassador.GetPendingRemoteDeletes();
		FireWeaponData[] pendingFires = this.lvcUnityAmbassador.GetPendingRemoteFires();
		DetonateMunitionData[] pendingDetonations = this.lvcUnityAmbassador.GetPendingRemoteDetonations();
		
		// we process the events in the following order: Creates - Updates - Deletes
		HandleCreations( pendingCreates );
		HandleUpdates( pendingUpdates );
		HandleDeletes( pendingDeletes );

		// Now that all the entities have been handled, we can process weapon fires and 
		// detonation events
		HandleFires( pendingFires );
		HandleDetonations( pendingDetonations );
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// LVC Game Related Methods //////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * Utility method to carry out tasks required for entity creation events
	 * 
	 * @param creations the data for the entity creation events
	 */
	private void HandleCreations( EntityData[] creations )
	{
		foreach( EntityData entityData in creations )
		{
			string gameType = entityData.id.gameType;
			// try to obtain the prefab from the asset manager
			GameObject prefab = AssetManager.GetPrefab( gameType );
			GameObject unityRepresentation;
			if( prefab )
			{
				unityRepresentation = Instantiate( prefab, UnityEngine.Vector3.zero, new Quaternion() ) as GameObject;
			}
			else
			{
				// fall back on the default prefab, if defined, otherwise just use a cube
				Debug.LogWarning( "External CREATE of "+ gameType + " - could not find prefab for '"+gameType+"'. Using default.");
				if( defaultPrefab != null )
					unityRepresentation = Instantiate( defaultPrefab, UnityEngine.Vector3.zero, new Quaternion() ) as GameObject;
				else
					unityRepresentation = GameObject.CreatePrimitive( PrimitiveType.Cube );
			}
			
			// add to our tracking of entity data ==> game objects
			this.lvcUnityAmbassador.RegisterUnityRepresentation( entityData, unityRepresentation );

			// make this the parent of the new unity object - keeps things organised. This isn't so
			// inmportant in standalone gameplay, but when creating the game we can end up with things 
			// piling up in the heirarchy view of the editor, making it difficult to track things.
			unityRepresentation.transform.parent = gameObject.transform;
			
			// update the representation so it's in the right place, pointing in the right direction
			AssignEntityDataToGameObject( entityData, unityRepresentation );
			
			// If we need to add any extra handlers of entitydata for this entity, we should do it now.
			// This might be necessary in the case of a tank, where we not only need to position the tank
			// itself, but also need to update the rotation of its turret - the turret rotation handling 
			// is not handled by the basic update, so an specialised handler would be required.
			/*
			if( gameType == LVCGameTypes.EXAMPLE )
			{
				unityRepresentation.AddComponent<ExampleExtraUpdateHandler>();
				unityRepresentation.AddComponent<ExampleExtraFireHandler>();
			}
			*/
		}
			
		if( showLogging && creations.Length>0 )
			Debug.Log( creations.Length + " External Creation Events Handled." );		
	}
	
	/**
	 * Utility method to carry out tasks required for entity update events
	 * 
	 * @param updates the data for the entity update events
	 */
	private void HandleUpdates( EntityData[] updates )
	{
		foreach( EntityData entityData in updates )
		{
			GameObject unityRepresentation = this.lvcUnityAmbassador.GameObjectForLvcId( entityData.id.instance );
			if( unityRepresentation != null )
			{
				AssignEntityDataToGameObject( entityData, unityRepresentation );

				// check for any entity type specific handling for updates
				LVCExtraUpdateHandler extraUpdateHandler = unityRepresentation.GetComponent<LVCExtraUpdateHandler>();
				if( extraUpdateHandler!=null )
				{
					// there's additional work to be done using the update data - do that now.
					extraUpdateHandler.HandleExtra( entityData, unityRepresentation );
				}
			}
		}
		
		if(showLogging && updates.Length>0)
			Debug.Log(updates.Length + " External Update Events Handled.");
	}
	
	/**
	 * Utility method to carry out tasks required for entity deletion events
	 * 
	 * @param deletions the data for the entity deletion events
	 */
	private void HandleDeletes( long[] deletions )
	{
		foreach( long id in deletions )
		{
			GameObject unityRepresentation = this.lvcUnityAmbassador.GameObjectForLvcId( id );
			if( unityRepresentation != null )
			{
				this.lvcUnityAmbassador.DeregisterUnityRepresentation( id );
				
				Destroy( unityRepresentation );
			}
		}
		
		if(showLogging && deletions.Length>0)
			Debug.Log(deletions.Length + " External Deletion Events Handled.");
	}
	
	/**
	 * Utility method to carry out tasks required for weapon fire events
	 * 
	 * @param fires the data for the weapon fire events
	 */
	private void HandleFires( FireWeaponData[] fires )
	{
		// there's nothing really that we need to do here (unless we animate the bullet itself...?)
		foreach( FireWeaponData fireWeaponData in fires )
		{
			GameObject unityRepresentation = this.lvcUnityAmbassador.GameObjectForLvcId( fireWeaponData.targeting.firingId );
			if( unityRepresentation!=null )
			{
				// check for any entity type specific handling for firing
				LVCExtraFireHandler extraFireHandler = unityRepresentation.GetComponent<LVCExtraFireHandler>();
				if( extraFireHandler!=null )
				{
					// there's additional work to be done using the fire data - do that now.
					extraFireHandler.HandleFire( fireWeaponData, unityRepresentation );
				}
			}
		}

		if(showLogging && fires.Length>0)
			Debug.Log(fires.Length + " External Fire Events Handled.");
	}
	
	/**
	 * Utility method to carry out tasks required for detonation events. These are *received* detonations - that is,
	 * they have come into Unity from an external source, so we need to make sure any internal entities (i.e., entities 
	 * "puppeted" by Unity) are affected appropriately (i.e., damage, appearance etc).
	 * 
	 * @param detonations the data for the detonation events
	 */
	private void HandleDetonations( DetonateMunitionData[] detonations )
	{
		foreach( DetonateMunitionData detonateMunitionData in detonations )
		{
			// Log detonation events if necessary...
			if( showLogging )
			{
				LVCPair<LVCGame.EntityData, GameObject> entityDataAndGameObject = this.lvcUnityAmbassador.GameObjectAndEntityDataForLvcId( detonateMunitionData.targeting.firingId );
				if( entityDataAndGameObject != null )
				{
					EntityData entityData = entityDataAndGameObject.A;
					Debug.Log( entityData.id.lvcType+"["+entityData.id.marking+"] detonated "+LVCUtils.DisplayFormatted(detonateMunitionData) );
				}
				else
				{
					Debug.Log( "Indirect fire of "+detonateMunitionData.descriptor.quantity+" rounds of "+detonateMunitionData.targeting.munitionType+"." );
				}
			}
			
			// handle the detonation event and affect entities as needed
			LVCGame.Vector3 lvcPosition = LVCUtils.llaToLtp( detonateMunitionData.targeting.position );
			UnityEngine.Vector3 unityPosition = LVCUtils.LVCGameCoords_to_UnityCoords( lvcPosition );
			string munitionTypeName = detonateMunitionData.targeting.munitionType;
			GameObject detonationPrefab = null; 
			if( munitionDetonationMap.TryGetValue( munitionTypeName, out detonationPrefab ) )
			{
				GameObject prefab = Instantiate( detonationPrefab, unityPosition, Quaternion.identity ) as GameObject;
				// make this the parent of the new prefab - keeps things organised, so we don't get prfeabs popping 
				// up in the "root" tree
				prefab.transform.parent = gameObject.transform;
				// Note that an actual application may wish to ensure that ground based detonation prefabs are ground
				// clamped, but that aerial weapons are not.
				
				// Work out which entities have been hit by the explosion
				// Note that the damage effect level and effective radius is hard coded. An actual application 
				// would probably need to make this dependent on the munition (and possibly the target 
				// vulerability to munition). Here we use a very simplistic damage model.
				float maxMunitionDamageEffect = 0.5F; 
				float maxEffectRadius = 10.0f;
				
				LVCPair<BasicLVCEntity, float>[] affectedEntities = lvcUnityAmbassador.GetAffectedEntities( unityPosition, maxEffectRadius );
				foreach( LVCPair<BasicLVCEntity, float> affectedEntity in affectedEntities ) 
				{
					BasicLVCEntity basicLvcEntity = affectedEntity.A; // entity affected by detonation
					float distance = affectedEntity.B; // distance of affected entity from from detonation
					
					// decrease damage based on inverse square law of distance of target from detonation location
					float actualDamageEffect = LVCUtils.SquareDecay( maxMunitionDamageEffect, maxEffectRadius, distance );
					
					EntityDataManager entityDataManager = basicLvcEntity.GetEntityDataManager();
					float damage = LVCUtils.ConstrainValue( entityDataManager.GetDamage()+actualDamageEffect, 0.0F, 1.0F ); 
					entityDataManager.SetDamage( damage );
				}
			}
			else
			{
				if( smokeMunitionPrefab != null )
				{
					string[] nameAndColor = munitionTypeName.Split ('-');
					if( nameAndColor.Length==2 )
					{
						Color smokeColor = Color.black;
						if( smokeMunitionColorMap.TryGetValue(nameAndColor[0], out smokeColor) )
						{
							GameObject prefab = Instantiate( smokeMunitionPrefab, unityPosition, Quaternion.identity ) as GameObject;
							// make this the parent of the new prefab - keeps things organised.
							prefab.transform.parent = gameObject.transform;
							// set the smoke color
							Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
							foreach( Renderer renderer in renderers )
								foreach( Material material in renderer.materials )
									material.color = smokeColor;
							// make sure the flare is on the ground, but don't change orientation
							DoGroundClamping( prefab, false );
						}
					}
				}
			}
		}
		
		if(showLogging && detonations.Length>0)
			Debug.Log(detonations.Length + " External Detonation Events Handled.");		
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Utility/Convenience Methods ////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * A utility function which applies the basic information contained in an LVC Game 
	 * EntityData structure to the appropriate properties of a Unity GameObject, such as
	 * position, orientation and so on.
	 * 
	 * @param entityData the LVC Game information which should be applied to the Unity game object
	 * @param gameObject the Unity game object to which the LVC Game information should be applied
	 */
	private void AssignEntityDataToGameObject( EntityData entityData, GameObject gO )
	{
		// position ----------------------------------------------------------------
		// AXIS (+ve): | LVC | UNITY
		//-------------+-----+-------
		// NORTH       |  X  |   X
		// EAST        |  Y  |   Z
		// VERTICAL    |  Z  |   Y
		
		// The x/y values in entityData.physics.position are lat/long in radians 'distance' relative to the map 
		// origin (i.e., not "absolute" lat/long on the surface of the Earth. Likewise the z value is the hieght
		// relative to the origin in meters.
		// Find map origin
		LVCGame.Vector3 worldOriginLLA = LVCUtils.GetLVCWorldOriginLLA();
		LVCGame.UTMCoordinate worldOriginUTM = LVCUtils.GetLVCWorldOriginUTM();
		// Make an adjusted position to asbolute lat/long, height in radians and meters
		LVCGame.Vector3 physicsPositionLLA = new LVCGame.Vector3( entityData.physics.position.x + worldOriginLLA.x,
		                                                          entityData.physics.position.y + worldOriginLLA.y,
		                                                          entityData.physics.position.z + worldOriginLLA.z );
		// convert absolute lat/long, height to local coordinates on the map using UTM
		LVCGame.UTMCoordinate physicsPositionUTM = LVCGame.LVCCoordinates.llaToUtm( ref physicsPositionLLA );
		UnityEngine.Vector3 unityPosition = new UnityEngine.Vector3( (float)(physicsPositionUTM.easting - worldOriginUTM.easting), 
		                                                             (float)(physicsPositionUTM.altitude - worldOriginUTM.altitude), 
		                                                             (float)(physicsPositionUTM.northing - worldOriginUTM.northing) );
		
		Quaternion unityOrientation = LVCUtils.LVCGameOrientation_to_UnityOrientation( entityData.physics.orientation );
		
		Rigidbody rigidbody = gO.GetComponent<Rigidbody>();
		if( rigidbody == null )
		{
			// simple movement - we just update the position and orientation, which results 
			// in accurate but "jerky" movement of the entity
			gO.transform.position = unityPosition;
			gO.transform.rotation = unityOrientation;
		}
		else
		{
			// we have a rigidbody, so we can use the Unity physics engine to smooth some of 
			// the movement and rotation out a bit
			LVCGame.Vector3 lvcVelocity = entityData.physics.worldVelocity;		
			UnityEngine.Vector3 unityVelocity = new UnityEngine.Vector3( (float)lvcVelocity.x, 
																		 (float)lvcVelocity.z, 
																		 (float)lvcVelocity.y );
			LVCGame.Vector3 lvcAngularVelocity = entityData.physics.bodyAngularVelocity;		
			UnityEngine.Vector3 unityAngularVelocity = new UnityEngine.Vector3( -(float)LVCUtils.Radians2Degrees(lvcAngularVelocity.z), 
																		         (float)LVCUtils.Radians2Degrees(lvcAngularVelocity.x), 
																		        -(float)LVCUtils.Radians2Degrees(lvcAngularVelocity.y) );
			rigidbody.velocity = unityVelocity;
			rigidbody.angularVelocity = unityAngularVelocity;
			
			rigidbody.position = unityPosition;
			rigidbody.rotation = unityOrientation;
		}
		
		// ground clamping for land entities ---------------------------------------
		if( LVCUtils.IsLandEntity(entityData) )
			DoGroundClamping( gO, true );
		
		// change colour of entity to indicate damage level ------------------------
		// Note that an actual application would probably want to represent damage in a 
		// more sophisticated way than that shown here. We simply gradually turn the
		// respresentation black, proportional to the amount of damage sustained.
		float damage = entityData.properties.damage;
		if( damage > 0.0F )
		{
			Color damageColor = Color.Lerp( DAMAGE_000, DAMAGE_100, damage );
			
			Renderer[] renderers = gO.GetComponentsInChildren<Renderer>();
			foreach( Renderer renderer in renderers )
				foreach( Material material in renderer.materials )
					material.color = damageColor;
		}
	}
	
	/**
	 * This utility method ensures that the GameObject passed in is "clamped" to the ground
	 * (i.e. the terrain) at that point, regardless of the Y (vertical) position originally
	 * assigned to it. This is useful for correcting small errors between the remote (LVC)
	 * terrain and the Unity terrain. It matches the entity's position and orientation to 
	 * the height and orientation of the terrain at its current position.
	 * 
	 * @param gO the GameObject instance which needs to be ground clamped
	 * @param alignNormal if true, the game object's "normal" (i.e., it's vertical axis) will
	 *        also be oriented so that the entity sits "flat" on the ground, matching how the
	 *        terrain is tilted underneath it (most suitable for vehicles).
	 *        If false, the game object will remain in whatever orientation it is currently in
	 *        (most suitable for smoke flares, bipedal lifeforms, etc)
	 */
	private void DoGroundClamping( GameObject gO, bool alignNormal )
	{
		// what terrain is being used?
		Terrain terrain = Terrain.activeTerrain;
		if( terrain != null )
		{
			// where is the entity now?
			Rigidbody rigidbody = gO.GetComponent<Rigidbody>();
			UnityEngine.Vector3 position = (rigidbody==null)?gO.transform.position:rigidbody.position;
		
			// sample the terrain height, and add it to the "base" height of the terrain
			// and figure out the Y (vertical) coordinate of the terrain at that point,
			// plus 1 centimeter of height, which is basically invisible but gives enough
			// "play" to be realistic. It also helps with the raycast we do next to find
			// out the orientation of the ground under the entity.
			position.y = terrain.transform.position.y + terrain.SampleHeight( position ) + 0.01F;
			
			// move the origin of the entity to the ground
			if( rigidbody!=null )
				rigidbody.position = position;
			else
				gO.transform.position = position;
			
			if( alignNormal )
			{
				// Also match tilt of the entity to the terrain tilt
				RaycastHit raycastHit;
				if ( Physics.Raycast(position, -UnityEngine.Vector3.up, out raycastHit) )
				{
					Quaternion groundTilt = Quaternion.FromToRotation( UnityEngine.Vector3.up, raycastHit.normal );
					
					if(rigidbody!=null)
						rigidbody.rotation = groundTilt * rigidbody.rotation;
					else
						gO.transform.rotation = groundTilt * gO.transform.rotation;
				}
			}
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
}
