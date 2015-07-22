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

using System.Collections;

using UnityEngine;
using LVCGame;

/**
 * This is a utility calss which encapsulates the LVC entity data for 
 * an Unity GameObject representation of an LVC entity.
 * 
 * It should be used as a proxy to communicate entity state to a LVC
 * simulation system. 
 * 
 * Refer to BasicLVCEntity for example usage.
 */
public class EntityDataManager 
{
	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	
	//----------------------------------------------------------
	//                   ENUMERATIONS
	//----------------------------------------------------------
	private enum LVCGameState
	{
		UNINTIALISED=0,
		CREATED,
		DELETED,
	};
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------
	private LVCGame.EntityData entityData;
	private LVCUnityAmbassador lvcUnityAmbassador;
	private LVCGame.ILVCClient lvcClient;
	private LVCGameState lvcGameState;
	
	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	
	public EntityDataManager( string gameType, string marking, LVCGame.UnitForceType force )
	{
		this.lvcUnityAmbassador = LVCUnityAmbassador.GetInstance();
		this.lvcClient = lvcUnityAmbassador.GetLVCClient();
		
		// initialise the entity data container which will hold salient details about 
		// the entity state which we want to send in LVC updates
		entityData = new LVCGame.EntityData();
		entityData.id = new LVCGame.EntityID();
		entityData.physics = new LVCGame.EntityPhysics();
		entityData.properties = new LVCGame.EntityProperties();
		entityData.properties.force = force;
		
		// assign a unique ID number to this entity
		entityData.id.instance = lvcUnityAmbassador.GetNextEntityID();
		
		// initialise types and markings
		entityData.id.gameType = gameType;
		entityData.id.marking = marking; 
		
		lvcGameState = LVCGameState.UNINTIALISED;
	}

	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	/**
	 * Creates the entity in the LVC Game world - until this is called, LVC Game
	 * does not consider the entity to "exist"
	 */
	public void Create( GameObject gameObject )
	{
		lock( this )
		{
			if( lvcGameState == LVCGameState.UNINTIALISED )
			{
				this.lvcUnityAmbassador.Register( ref this.entityData, gameObject );
				lvcGameState = LVCGameState.CREATED;
			}
		}
	}

	/**
	 * Determine if the entity has been created in the LVC Game world
	 */
	public bool IsCreated()
	{
		bool result = false;
		lock( this )
		{
			result = ( lvcGameState == LVCGameState.CREATED );
		}
		return result;
	}

	/**
	 * Causes an update to be sent into the LVC Game world. If Create() has not yet 
	 * been called, or Destroy() has been called, this will have no effect.
	 */
	public void Update()
	{
		lock( this )
		{
			if( lvcGameState == LVCGameState.CREATED )
				this.lvcClient.UpdateEntity( ref this.entityData );
		}
	}
	
	/**
	 * Causes an weapon fire notification to be sent into the LVC Game world. If 
	 * Create() has not yet been called, or Destroy() has been called, this will 
	 * have no effect.
	 * 
	 * @param lvcMunitionType the LVC Game munition type name
	 * @param origin the location from which the munition originated
	 * @param velocity the velocity of the munition
	 * @param quantity (optional) the number of rounds fired in this "burst" (if unspecified, 1 is used)
	 * @param rate (optional) the rate of fire in rounds per minute
	 * @param warhead (optional) the DIS warhead type of the fired munition
	 * @param fuse (optional) the DIS fuse type of the fired munition
	 * @param targetId (optional) the ID of the entity targeted by this munition fire
	 */
	public void FireWeapon( string lvcMunitionType, UnityEngine.Vector3 origin, UnityEngine.Vector3 velocity,
							uint quantity, uint rate, 
						    LVCGame.WarHeadType warhead, 
							LVCGame.FuseType fuse,
							long targetId
						  )
	{
		lock( this )
		{
			if( lvcGameState == LVCGameState.CREATED )
			{
				LVCGame.FireWeaponData fireWeaponData = lvcUnityAmbassador.MakeFireWeaponData(
					this.entityData.id.instance,
					lvcMunitionType, origin, velocity,
					quantity, rate, 
					warhead, fuse,
					targetId
				);
			
				this.lvcClient.FireWeapon( ref fireWeaponData );
			}
		}		
	}
	
	/**
	 * Causes an munition detonation notification to be sent into the LVC Game 
	 * world. If Create() has not yet been called, or Destroy() has been called, 
	 * this will have no effect.
	 * 
	 * @param lvcMunitionType the LVC Game munition type name
	 * @param position the location at which the detonation occurs
	 * @param velocity the velocity of the munition - this is the "approach vector" of the munition
	 *        as viewed from the detonation location
	 * @param quantity (optional) the number of rounds fired in this "burst" (if unspecified, 1 is used)
	 * @param rate (optional) the rate of fire in rounds per minute
	 * @param warhead (optional) the DIS warhead type of the detonated munition
	 * @param fuse (optional) the DIS fuse type of the detonated munition
	 * @param detonation (optional) the DIS detonation type of the detonated munition
	 * @param targetId (optional) the ID of the entity targeted by this detonated
	 * @param relativePosition (optional) the relative position of the detonation to the target
	 */
	public void DetonateMunition( string lvcMunitionType, UnityEngine.Vector3 position, UnityEngine.Vector3 velocity,
								  uint quantity, uint rate, 
						    	  LVCGame.WarHeadType warhead, 
								  LVCGame.FuseType fuse,
								  LVCGame.DetonationType detonation,
								  long targetId,
								  UnityEngine.Vector3 relativePosition
								)
	{
		lock( this )
		{
			if( lvcGameState == LVCGameState.CREATED )
			{
				LVCGame.DetonateMunitionData detonateMunitionData = lvcUnityAmbassador.MakeDetonateMunitionData(
					this.entityData.id.instance,
					lvcMunitionType, position, velocity,
					quantity, rate, 
					warhead, fuse,
					detonation,
					targetId,
					relativePosition
				);
				
				this.lvcClient.DetonateMunition( ref detonateMunitionData );
			}
		}		
	}
	
	/**
	 * Causes the entity to be removed from the LVC Game world. After this is called,
	 * no further updates will be sent.
	 */
	public void Delete()
	{
		lock( this )
		{
			if( lvcGameState == LVCGameState.CREATED )
			{
				this.lvcUnityAmbassador.Deregister( ref this.entityData );
				lvcGameState = LVCGameState.DELETED;
			}
		}
	}
	
	/**
	 * Determine if the entity has been destroyed in the LVC Game world
	 */
	public bool IsDeleted()
	{
		bool result = false;
		lock( this )
		{
			result = ( lvcGameState == LVCGameState.DELETED );
		}
		return result;
	}

	/**
	 * A utility function which returns a formatted formatted string containing 
	 * details about the information contained in the entity data (suitable for
	 * logging purposes).
	 */
	public string DisplayFormatted()
	{
		string result;
		lock( this )
		{
			result = LVCUtils.DisplayFormatted( this.entityData );
		}
		return result;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * Obtain the game type of the entity managed by this instance
	 * 
	 * @return the game type
	 */
	public string GetGameType()
	{
		// no lock required - the game type is read-only data (i.e., it never changes)
		return this.entityData.id.gameType;
	}
	
	/**
	 * Obtain the LVC ID of the entity managed by this instance
	 * 
	 * @return the entity LVC ID number
	 */
	public long GetLVCId()
	{
		// no lock required - the entity ID number is read-only data (i.e., it never changes)
		return this.entityData.id.instance;
	}
	
	/**
	 * Obtain the LVC type of the entity managed by this instance
	 * 
	 * @return the LVC type
	 */
	public string GetLVCType()
	{
		// no lock required - the LVC type is read-only data (i.e., it never changes)
		return this.entityData.id.lvcType;
	}
	
	/**
	 * Obtain the LVC marking of the entity managed by this instance
	 * 
	 * @return the LVC marking
	 */
	public string GetLVCMarking()
	{
		// no lock required - the LVC marking is read-only data (i.e., it never changes)
		return this.entityData.id.marking;
	}
	
	//------------------------------------------------------------------------------------------
	// POSITION
	//------------------------------------------------------------------------------------------
	/**
	 * Set the position of the LVC entity using a UnityEngine Vector3 containing LTP 
	 * style coordinates.
	 * 
	 * @param position a UnityEngine.Vector3 containing LTP position information
	 */
	public void SetPosition( UnityEngine.Vector3 xyz )
	{
		lock( this )
		{
			// AXIS (+ve): | LVC | UNITY
			//-------------+-----+-------
			// NORTH       |  X  |   Z
			// EAST        |  Y  |   X
			// VERTICAL    |  Z  |   Y
			
			
			// The x/y values in entityData.physics.position are lat/long in radians 'distance' relative to the map 
			// origin (i.e., not "absolute" lat/long on the surface of the Earth. Likewise the z value is the hieght
			// relative to the origin in meters.
			// Find map origin
			LVCGame.UTMCoordinate worldOriginUTM = LVCUtils.GetLVCWorldOriginUTM();
			LVCGame.UTMCoordinate entityUTM = new LVCGame.UTMCoordinate( worldOriginUTM.easting + xyz.x,
			                                                             worldOriginUTM.northing + xyz.z,
			                                                             worldOriginUTM.altitude + xyz.y,
			                                                             worldOriginUTM.zone,
			                                                             worldOriginUTM.hemisphere);
			
			// convert to absolute lat/long, height
			LVCGame.Vector3 entityLLA = LVCGame.LVCCoordinates.utmToLla( ref entityUTM );
			
			// Make an adjusted position to relative lat/long, height in radians, meters from origin
			LVCGame.Vector3 worldOriginLLA = LVCUtils.GetLVCWorldOriginLLA();
			this.entityData.physics.position.x = entityLLA.x - worldOriginLLA.x;
			this.entityData.physics.position.y = entityLLA.y - worldOriginLLA.y;
			this.entityData.physics.position.z = entityLLA.z - worldOriginLLA.z;
		}
	}
	
	//------------------------------------------------------------------------------------------
	// ORIENTATION
	//------------------------------------------------------------------------------------------
	/**
	 * Set the orientation of the entity using a Unity Quaternion instance
	 * 
	 * @param orientation a Quaternion instance
	 */
	public void SetOrientation( UnityEngine.Quaternion orientation )
	{
		lock( this )
		{
			this.entityData.physics.orientation = LVCUtils.UnityOrientation_to_LVCGameOrientation( orientation );
		}
	}
	
	//------------------------------------------------------------------------------------------
	// VELOCITY
	//------------------------------------------------------------------------------------------
	/**
	 * Get the velocity of the entity
	 * 
	 * @return velocity a UnityEngine.Vector3 containing the velocity information
	 */
	public void SetWorldVelocity( UnityEngine.Vector3 velocity )
	{
		lock( this )
		{
			// AXIS (+ve): | LVC | UNITY
			//-------------+-----+-------
			// NORTH       |  X  |   X
			// EAST        |  Y  |   Z
			// VERTICAL    |  Z  |   Y
			this.entityData.physics.worldVelocity.x = velocity.x;
			this.entityData.physics.worldVelocity.y = velocity.z;
			this.entityData.physics.worldVelocity.z = velocity.y;
		}
	}
	
	//------------------------------------------------------------------------------------------
	// ACCELERATION
	//------------------------------------------------------------------------------------------
	/**
	 * Set the acceleration of the entity
	 * 
	 * @param acceleration a UnityEngine.Vector3 containing the acceleration information
	 */
	public void SetWorldAcceleration( UnityEngine.Vector3 acceleration )
	{
		lock( this )
		{
			// AXIS (+ve): | LVC | UNITY
			//-------------+-----+-------
			// NORTH       |  X  |   X
			// EAST        |  Y  |   Z
			// VERTICAL    |  Z  |   Y
			this.entityData.physics.worldAcceleration.x = acceleration.x;
			this.entityData.physics.worldAcceleration.y = acceleration.z;
			this.entityData.physics.worldAcceleration.z = acceleration.y;
		}
	}
	
	//------------------------------------------------------------------------------------------
	// ANGULAR VELOCITY
	//------------------------------------------------------------------------------------------
	/**
	 * Set the angular velocity of the entity (in degrees per second) 
	 * using a Unity Vector3
	 * 
	 * @param velocity the angular velocity in degrees per second
	 */
	public void SetBodyAngularVelocity( UnityEngine.Vector3 velocity )
	{
		// AXIS (+ve): | UNITY | LVC
		//-------------+-------+-------
		// ROLL        |  X    |  -X
		// YAW         |  Y    |   Z
		// PITCH       |  Z    |   Y
		this.entityData.physics.bodyAngularVelocity.x = -LVCUtils.Degrees2Radians(velocity.x); // ROLL
		this.entityData.physics.bodyAngularVelocity.y = LVCUtils.Degrees2Radians(velocity.z);  // YAW
		this.entityData.physics.bodyAngularVelocity.z = LVCUtils.Degrees2Radians(velocity.y);  // PITCH
	}
	
	//------------------------------------------------------------------------------------------
	// UNIT FORCE TYPE
	//------------------------------------------------------------------------------------------
	/**
	 * Set the unit force type of the entity
	 * 
	 * @param unitForceType the force of the entity
	 */
	public void SetForce( LVCGame.UnitForceType unitForceType )
	{
		lock( this )
		{
			this.entityData.properties.force = unitForceType;
		}
	}
	
	//------------------------------------------------------------------------------------------
	// DAMAGE
	//------------------------------------------------------------------------------------------
	/**
	 * Get the damage of the entity
	 * 
	 * @return the damage of the entity
	 */
	public float GetDamage()
	{
		float damage = 0.0f;
		lock( this )
		{
			damage = this.entityData.properties.damage;
		}
		return damage;
	}
	
	/**
	 * Set the damage of the entity
	 * 
	 * @param damage the damage of the entity
	 */
	public void SetDamage( float damage )
	{
		lock( this )
		{
			this.entityData.properties.damage = damage;
		}
	}
	
	//------------------------------------------------------------------------------------------
	// APPEARANCE
	//------------------------------------------------------------------------------------------
	/**
	 * Set the appearance of the entity
	 * 
	 * @param appearance the appearance of the entity
	 */
	public void SetAppearance( uint appearance )
	{
		lock( this )
		{
			this.entityData.properties.appearance = appearance;
		}
	}
	
	/**
	 * Obtain the appearance of the entity
	 * 
	 * @return the appearance of the entity
	 */
	public uint GetAppearance()
	{
		uint appearance = 0;
		lock( this )
		{
			appearance = this.entityData.properties.appearance;
		}
		return appearance;
	}
	
	//------------------------------------------------------------------------------------------
	// LIGHTS
	//------------------------------------------------------------------------------------------
	/**
	 * Set the lights of the entity
	 * 
	 * @param lights the lights of the entity
	 */
	public void SetLights( uint lights )
	{
		lock( this )
		{
			this.entityData.properties.lights = lights;
		}
	}
	
	//------------------------------------------------------------------------------------------
	// STANCE
	//------------------------------------------------------------------------------------------
	/**
	 * Set the stance type of the entity
	 * 
	 * @param stanceType the stance of the entity
	 */
	public void SetStance( LVCGame.StanceType stanceType )
	{
		lock( this )
		{
			this.entityData.properties.stance = stanceType;
		}
	}
	
	/**
	 * Obtain the stance type of the entity
	 * 
	 * @return the stance of the entity
	 */
	public LVCGame.StanceType GetStance()
	{
		LVCGame.StanceType result = LVCGame.StanceType.None;
		lock( this )
		{
			result = this.entityData.properties.stance;
		}
		return result;
	}
	
	//------------------------------------------------------------------------------------------
	// PRIMARY WEAPON POSTURE
	//------------------------------------------------------------------------------------------
	/**
	 * Set the primary weapon posture of the entity
	 * 
	 * @param primaryWeaponPosture the primary weapon posture of the entity
	 */
	public void SetPrimaryWeaponPosture( LVCGame.WeaponPostureType primaryWeaponPosture )
	{
		lock( this )
		{
			this.entityData.properties.prWeaponPosture = primaryWeaponPosture;
		}
	}
	
	/**
	 * Get the primary weapon posture of the entity
	 * 
	 * @return the primary weapon posture of the entity
	 */
	public LVCGame.WeaponPostureType GetPrimaryWeaponPosture(  )
	{
		LVCGame.WeaponPostureType result = LVCGame.WeaponPostureType.None;
		lock( this )
		{
			result = this.entityData.properties.prWeaponPosture;
		}
		return result;
	}
	
	//------------------------------------------------------------------------------------------
	// SECONDARY WEAPON POSTURE
	//------------------------------------------------------------------------------------------
	/**
	 * Set the secondary weapon posture of the entity
	 * 
	 * @param secondaryWeaponPosture the secondary weapon posture of the entity
	 */
	public void SetSecondaryWeaponPosture( LVCGame.WeaponPostureType secondaryWeaponPosture )
	{
		lock( this )
		{
			this.entityData.properties.sndWeaponPosture = secondaryWeaponPosture;
		}
	}
}