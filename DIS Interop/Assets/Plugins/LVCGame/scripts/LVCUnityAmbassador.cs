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
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using UnityEngine;
using LVCGame;

/**
 * This class is used to obtain a reference to the LVC Client (through which notifications of 
 * entity creation, updates and deletions may be sent).
 */
public class LVCUnityAmbassador : ILVCEvents
{
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------
	// singleton ambassador instance
	private static LVCUnityAmbassador lvcUnityAmbassador;
	
	private ILVCClient lvcClient;
	private Thread simTickThread;
	private SimTicker simTicker;
	// this keeps track of entities created from within Unity
	private Dictionary<long, GameObject> internalEntityRegistry;
	// this allows us to look up a Unity GameObject (and associated EntityData) with an LVC ID
	private Dictionary<long, LVCPair<LVCGame.EntityData, GameObject>> externalEntityRegistry;
	// this allows us to look up an LVC ID using a Unity GameObject
	private Dictionary<GameObject, long> gameObjectToLvcIdMap;
	// used to assign a unique ID value to entities and targeting (fire/detonation) events
	private long nextEntityID = 0;
	private uint nextTargetingID = 0;
	
	private List<LVCGame.EntityData> pendingExternalCreates;
	private List<LVCGame.EntityData> pendingExternalUpdates;
	private List<long> pendingExternalDeletes;
	private List<LVCGame.FireWeaponData> pendingExternalFires;
	private List<LVCGame.DetonateMunitionData> pendingExternalDetonations;
	
	private Object nextEntityIdLock = new Object();
	private Object nextTargetingIdLock = new Object();
	private Object internalEntityRegistryLock = new Object();
	private Object externalEntityRegistryLock = new Object();
	private Object gameObjectToLvcIdMapLock = new Object();
	private Object pendingExternalCreateLock = new Object();
	private Object pendingExternalUpdateLock = new Object();
	private Object pendingExternalDeleteLock = new Object();
	private Object pendingExternalFireLock = new Object();
	private Object pendingExternalDetonationLock = new Object();
	
	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	private LVCUnityAmbassador () 
	{
		init ();
	}
	
	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	/**
	 * Initialisation and setup. This is only used internally.
	 */
	private void init() 
	{
		internalEntityRegistry = new Dictionary<long, GameObject>();
		externalEntityRegistry = new Dictionary<long, LVCPair<LVCGame.EntityData, GameObject>>();
		gameObjectToLvcIdMap = new Dictionary<GameObject, long>();
		
		pendingExternalCreates = new List<LVCGame.EntityData>();
		pendingExternalUpdates = new List<LVCGame.EntityData>();
		pendingExternalDeletes = new List<long>();
		pendingExternalFires = new List<LVCGame.FireWeaponData>();
		pendingExternalDetonations = new List<LVCGame.DetonateMunitionData>();
		
		// TODO - these initialisation values should not be hard coded
		string lvcConfigPath = LVCUtils.GetLVCConfigPath();
		lvcClient = new LVCClient( LVCUtils.CLIENT_TYPE, 
							       LVCUtils.LocalizePath(lvcConfigPath+"/LVCGame.log") );
		lvcClient.Initialize( LVCUtils.LocalizePath(lvcConfigPath) );
		
		lvcClient.SetEventsHandlerCallbacks( this, false );
		
		lvcClient.Start();
		
		simTicker = new SimTicker( ref lvcClient, 1.0f/30.0f);
		simTickThread = new Thread( new ThreadStart( simTicker.ThreadProc ));
		simTickThread.Start ();
	}
	
	/**
	 * Obtain a reference to the LVCClient instance which is responsible for sending 
	 * and receiving simulation events
	 */
	public ILVCClient GetLVCClient()
	{
		return this.lvcClient;
	}
	
	/**
	 * Obtain an entity ID which is unique in the context of this Unity instance. 
	 * All entities which are to be used in an LVC simulation must obtain their 
	 * IDs using this method in order to avoid clashes in the simulation.
	 * 
	 * @return a unique (in the context of this Unity instance) entity ID 
	 */
	public long GetNextEntityID()
	{
		lock( nextEntityIdLock )
		{
			return ++this.nextEntityID;
		}
	}
	
	/**
	 * Obtain a targeting event ID (for fires and detonations) which is unique 
	 * in the context of this Unity instance. All targeting events which are to 
	 * be used in an LVC simulation must obtain their IDs using this method in 
	 * order to avoid clashes in the simulation.
	 * 
	 * @return a unique (in the context of this Unity instance) targeting ID 
	 */
	public uint GetNextTargetingEventID()
	{
		lock( nextTargetingIdLock )
		{
			return ++this.nextTargetingID;
		}
	}
	
	public void RegisterUnityRepresentation( EntityData entityData, GameObject unityRepresentation )
	{
		lock( externalEntityRegistryLock )
		{
			externalEntityRegistry.Add( entityData.id.instance, new LVCPair<LVCGame.EntityData, GameObject>(entityData, unityRepresentation) );
		}
		lock( gameObjectToLvcIdMapLock )
		{
			gameObjectToLvcIdMap.Add( unityRepresentation, entityData.id.instance);
		}
	}
	
	public void DeregisterUnityRepresentation( long id )
	{
		LVCPair<LVCGame.EntityData, GameObject> entityDataAndGameObject = null;
		lock( externalEntityRegistryLock )
		{
			externalEntityRegistry.TryGetValue( id, out entityDataAndGameObject );
			externalEntityRegistry.Remove( id );
		}
		if( entityDataAndGameObject != null )
		{
			lock( gameObjectToLvcIdMapLock )
			{
				gameObjectToLvcIdMap.Remove( entityDataAndGameObject.B );
			}
		}
	}
	
	public void Register( ref EntityData entityData, GameObject gameObject )
	{
		lock( internalEntityRegistryLock )
		{
			if( !internalEntityRegistry.ContainsKey(entityData.id.instance) )
			{
				internalEntityRegistry.Add( entityData.id.instance, gameObject );
				lvcClient.CreateEntity( ref entityData );
			}
		}
	}
	
	public void Deregister( ref EntityData entityData )
	{
		lock( internalEntityRegistryLock )
		{
			if( internalEntityRegistry.ContainsKey(entityData.id.instance) )
			{
				long id = entityData.id.instance;
				internalEntityRegistry.Remove( id );
				lvcClient.DeleteEntity( id );
			}
		
			if( internalEntityRegistry.Count == 0)
				ShutDown();
		}
	}
	
	/**
	 * Shut down the LVC Client instance.
	 */
	public void ShutDown()
	{
		Debug.Log("Shutting down instance of LVC Game");
		
		if( simTickThread!= null )
		{
			simTicker.shutDown();
			simTickThread.Abort();
			simTickThread = null;
			simTicker = null;
		}
		
		if( lvcClient != null )
		{
			lvcClient.Tick(500);
			lvcClient.Stop();
			lvcClient = null;
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////// Remotely Triggered Event Helpers /////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	public EntityData[] GetPendingRemoteCreates()
	{
		EntityData[] pending;
		lock( pendingExternalCreateLock )
		{
			// copy the data in the pending event list to an array, which we return
			pending = pendingExternalCreates.ToArray();
			// we assume that whatever called this method *will* deal with the pending
			// events, and so clear our own records at this point.
			pendingExternalCreates.Clear();
		}
		return pending;
	}
	
	public EntityData[] GetPendingRemoteUpdates()
	{
		EntityData[] pending;
		lock( pendingExternalUpdateLock )
		{
			// copy the data in the pending event list to an array, which we return
			pending = pendingExternalUpdates.ToArray();
			// we assume that whatever called this method *will* deal with the pending
			// events, and so clear our own records at this point.
			pendingExternalUpdates.Clear();
		}
		return pending;
	}
	
	public long[] GetPendingRemoteDeletes()
	{
		long[] pending;
		lock( pendingExternalDeleteLock )
		{
			// copy the data in the pending event list to an array, which we return
			pending = pendingExternalDeletes.ToArray();
			// we assume that whatever called this method *will* deal with the pending
			// events, and so clear our own records at this point.
			pendingExternalDeletes.Clear();
		}
		return pending;
	}
	
	public FireWeaponData[] GetPendingRemoteFires()
	{
		FireWeaponData[] pending;
		lock( pendingExternalFireLock )
		{
			// copy the data in the pending event list to an array, which we return
			pending = pendingExternalFires.ToArray();
			// we assume that whatever called this method *will* deal with the pending
			// events, and so clear our own records at this point.
			pendingExternalFires.Clear();
		}
		return pending;
	}
	
	public DetonateMunitionData[] GetPendingRemoteDetonations()
	{
		DetonateMunitionData[] pending;
		lock( pendingExternalDetonationLock )
		{
			// copy the data in the pending event list to an array, which we return
			pending = pendingExternalDetonations.ToArray();
			// we assume that whatever called this method *will* deal with the pending
			// events, and so clear our own records at this point.
			pendingExternalDetonations.Clear();
		}
		return pending;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////// ILVCEvents Implementations ////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
    /**
     * Fires a create entity event. This is triggered as a result of an *incoming* LVC create
     * event triggered by an LVC Game external to this one, meaning that an new, equivalent Unity
     * game object should be created. For example, create a tank inside Unity to represent a 
     * remote VBS2 owned/controlled tank.
     * 
     * @param data Data associated with the event
     * @return true if the event was handled successfully
     */
    public bool CreateEntity( ref EntityData data )
	{
		lock( pendingExternalCreateLock )
		{
			// Debug.Log("External CREATE event received.");
			// we assume here that the creation will eventually succeed by assigning unique
			// entity ID to the received entity data
			data.id.instance = GetNextEntityID();
			
			// add the data as a pending external create. Since this event arrived "outside"
			// Unity's event/threading system, this will eventually be handled by the LVCHelper 
			// Monobehaviour during an Update() event.
			pendingExternalCreates.Add( data );
		}		
		return true;
	}
	
	/**
	 * Fires an update entity event. This is triggered as a result of an *incoming* LVC update
     * event triggered by an LVC Game external to this one, meaning that the equivalent Unity game
     * object should be updated. For example, update the tank inside Unity to which represents the 
     * remote VBS2 owned/controlled tank.
	 * 
     * @param data data associated with the event
	 * @return true if the event was handled successfully
	 */
    public bool UpdateEntity( ref EntityData data )
	{
		lock( pendingExternalUpdateLock )
		{
			// Debug.Log("External UPDATE event received.");
			// add the data as a pending external update. Since this event arrived "outside"
			// Unity's event/threading system, this will eventually be handled by the LVCHelper 
			// Monobehaviour during an Update() event.
			pendingExternalUpdates.Add( data );
		}		
		return true;
	}
	
	/**
	 * Fires a delete entity event. This is triggered as a result of an *incoming* LVC delete
     * event triggered by an LVC Game external to this one, meaning that the equivalent Unity game
     * object should be destroyed/removed. For example, delete the tank inside Unity to which 
     * represented the remote VBS2 owned/controlled tank.
	 * 
     * @param id Instance ID of the entity to delete
     * @returns true if the event was handled successfully
     */
    public bool DeleteEntity( long id )
	{
		lock( pendingExternalDeleteLock )
		{
			// Debug.Log("External DELETE event received.");
			// add the data as a pending external deletion. Since this event arrived "outside"
			// Unity's event/threading system, this will eventually be handled by the LVCHelper 
			// Monobehaviour during an Update() event.
			pendingExternalDeletes.Add( id );
		}		
		return true;
	}
	
	/**
     * Fires a fire weapon event. This is triggered as a result of an *incoming* LVC update
     * event triggered by an LVC Game external to this one, meaning that the equivalent Unity game
     * object should fire. For example, cause the tank inside Unity to which represents the 
     * remote VBS2 owned/controlled tank to fire.
     * 
     * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool FireWeapon( ref FireWeaponData data )
	{
		lock( pendingExternalFireLock )
		{
			// Debug.Log("External FIREWEAPON event received.");
			// add the data as a pending external deletion. Since this event arrived "outside"
			// Unity's event/threading system, this will eventually be handled by the LVCHelper 
			// Monobehaviour during an Update() event.
			pendingExternalFires.Add( data );
		}		
		return true;
	}
	
	/**
	 * Fires a detonate munition event. This is triggered as a result of an *incoming* LVC update
     * event triggered by an LVC Game external to this one, meaning that the equivalent Unity game
     * object should be detonate a munition.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool DetonateMunition( ref DetonateMunitionData data )	
	{
		lock( pendingExternalDetonationLock )
		{
			// Debug.Log("External DETONATEMUNITION event received.");
			// add the data as a pending external deletion. Since this event arrived "outside"
			// Unity's event/threading system, this will eventually be handled by the LVCHelper 
			// Monobehaviour during an Update() event.
			pendingExternalDetonations.Add( data );
		}		
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Laser Designator creation events are currently not handled by this
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool CreateLaserDesignator( ref LaserDesignatorData data )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Laser Designator update events are currently not handled by this
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool UpdateLaserDesignator( ref LaserDesignatorData data )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Laser Designator deletion events are currently not handled by this
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool DeleteLaserDesignator( long id )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Transmitter creation events are currently not handled by this
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool CreateTransmitter( ref TransmitterData data )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Transmitter update events are currently not handled by this
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool UpdateTransmitter( ref TransmitterData data )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Transmitter deletion events are currently not handled by this
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool DeleteTransmitter( long id )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Data events are currently not handled by this implementation, and
	 * simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool Data( ref Data data )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Set data events are currently not handled by this implementation, and
	 * simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool SetData( ref Data data )
	{
		// currently ignored
		return true;
	}	
	
	/**
	 * This is triggered as a result of an *incoming* LVC update event triggered by an LVC Game
	 * external to this one. Radio signal data events are currently not handled by this 
	 * implementation, and simply ignored.
	 * 
	 * @param data data associated with the event
     * @return true if the event was handled successfully
     */
    public bool RadioSignal( ref RadioSignalData data )
	{
		// currently ignored
		return true;
	}	
	
	////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////// Utility Methods /////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * A utility method which creates an fire weapon event data structure.
	 * 
	 * @param firingID ID of entity which fired the detonating munition
	 * @param lvcMunitionType the LVC Game munition type name
	 * @param origin the location from which the munition originated
	 * @param velocity the velocity of the munition
	 * @param quantity (optional) the number of rounds fired in this "burst" (if unspecified, 1 is used)
	 * @param rate (optional) the rate of fire in rounds per minute
	 * @param warhead (optional) the DIS warhead type of the fired munition
	 * @param fuse (optional) the DIS fuse type of the fired munition
	 * @param targetId (optional) the ID of the entity targeted by this munition fire
	 * 
	 * @return a populated LVCGame.FireWeaponData instance
	 */
	public LVCGame.FireWeaponData MakeFireWeaponData(
							long firingID,
							string lvcMunitionType, 
							UnityEngine.Vector3 origin, UnityEngine.Vector3 velocity,
							uint quantity, uint rate, 
						    LVCGame.WarHeadType warheadType, 
							LVCGame.FuseType fuseType,
							long targetId
						   )	
	{
		// convert Unity's LTP coordinates to lat/long/altitude
		LVCGame.Vector3 llaOrigin = LVCUtils.ltpToLla( LVCUtils.UnityCoords_to_LVCGameCoords(origin) );
		// convert Unity XYZ values to LVC axes
		LVCGame.Vector3 lvcVelocity = LVCUtils.UnityCoords_to_LVCGameCoords( velocity );
		
		LVCGame.Targeting targeting = new LVCGame.Targeting();
		targeting.eventId = GetNextTargetingEventID();
		targeting.firingId = firingID; // ID of entity which fired the weapon
		targeting.targetId = targetId; // ID of entity which is the target of the fire
		targeting.munitionId = 0; // unused - use munitionType field instead
		targeting.munitionType = lvcMunitionType; // LVC Game type name of the fired munition
		targeting.position = llaOrigin; // origin of the fired munition
		targeting.velocity = lvcVelocity; // velocity of the fired munition
		
		LVCGame.BurstDescriptor descriptor = new LVCGame.BurstDescriptor();
		descriptor.quantity = quantity; // amount of rounds fired in this burst
		descriptor.rate = rate; // rate of rounds fired in this burst in rounds per minute
		descriptor.warhead = warheadType; // warhead type
		descriptor.fuse = fuseType; // fuse type
		
		LVCGame.FireWeaponData fireWeaponData = new LVCGame.FireWeaponData();
		fireWeaponData.targeting = targeting;
		fireWeaponData.descriptor = descriptor;
		
		return fireWeaponData;
	}	
	
	/**
	 * A utility method which creates an munition detonation event data structure.
	 * 
	 * @param firingID ID of entity which fired the detonating munition
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
	 * 
	 * @return a populated LVCGame.DetonateMunitionData instance
	 */
	public LVCGame.DetonateMunitionData MakeDetonateMunitionData(
								  long firingID,
								  string lvcMunitionType, 
								  UnityEngine.Vector3 position, UnityEngine.Vector3 velocity,
								  uint quantity, uint rate, 
						    	  LVCGame.WarHeadType warheadType, 
								  LVCGame.FuseType fuseType,
								  LVCGame.DetonationType detonationType,
								  long targetId,
								  UnityEngine.Vector3 relativePosition
								)	
	{
		// convert Unity's LTP coordinates to lat/long/altitude
		LVCGame.Vector3 llaPosition = LVCUtils.ltpToLla( LVCUtils.UnityCoords_to_LVCGameCoords(position) );
		// convert Unity XYZ values to LVC axes
		LVCGame.Vector3 lvcVelocity = LVCUtils.UnityCoords_to_LVCGameCoords( velocity );
		LVCGame.Vector3 lvcRelativePosition = LVCUtils.UnityCoords_to_LVCGameCoords( relativePosition );
		
		// Construct the targeting data
		LVCGame.Targeting targeting = new LVCGame.Targeting();
		targeting.eventId = GetNextTargetingEventID();
		targeting.firingId = firingID; // ID of entity which fired the detonating munition
		targeting.targetId = targetId; // ID of entity which was the target of the detonating munition
		targeting.munitionId = 0; // unused - use munitionType instead
		targeting.munitionType = lvcMunitionType; // LVC Game type name of the detonating munition
		targeting.position = llaPosition; // location of the detonation munition
		targeting.velocity = lvcVelocity; // velocity of the "approach" of the detonating munition
		
		// Construct the burst descriptor data
		LVCGame.BurstDescriptor descriptor = new LVCGame.BurstDescriptor();
		descriptor.quantity = quantity; // amount of munitions detonated in this burst
		descriptor.rate = rate; // rate of munitions detonated in this burst in rounds per minute
		descriptor.warhead = warheadType; // warhead type
		descriptor.fuse = fuseType; // fuse type
		
		// construct the detonation data
		LVCGame.DetonateMunitionData detonateMunitionData = new LVCGame.DetonateMunitionData();
		detonateMunitionData.targeting = targeting;
		detonateMunitionData.descriptor = descriptor;
		detonateMunitionData.detonation = detonationType;
		detonateMunitionData.relativePosition = lvcRelativePosition;
		
		return detonateMunitionData;
	}
	
	/**
	 * Utility method to determine which entities have been affected by an event (usually a
	 * detonation)
	 * 
	 * @param epicenter the location of the event
	 * @param effectRadius the effect radius of the event
	 * @return an array of pairs consisting of the BasicLVCEntity of entities affected by the event 
	 *         paired with their distance from the event.
	 */
	public LVCPair<BasicLVCEntity, float>[] GetAffectedEntities( UnityEngine.Vector3 epicenter, float effectRadius )
	{
		// square of distance is cheaper for comparison
		float effectRadiusSquared = ( effectRadius * effectRadius );
		
		// Pairs are LVC ID -vs- distance from epicenter
		List<LVCPair<BasicLVCEntity, float>> affected = new List<LVCPair<BasicLVCEntity, float>>();
		lock( internalEntityRegistryLock )
		{
			foreach( KeyValuePair<long, GameObject> entry in internalEntityRegistry ) 
			{
				BasicLVCEntity basicLvcEntity = entry.Value.GetComponent<BasicLVCEntity>();
				if ( basicLvcEntity )
				{
					// start with "cheap" square of distance comparison
					float distanceSquared = (entry.Value.transform.position - epicenter).sqrMagnitude;
					if ( distanceSquared <= effectRadiusSquared )
					{
						// we know it's within the effect radius, do "expensive" square root calc now
						affected.Add( new LVCPair<BasicLVCEntity, float>(basicLvcEntity, Mathf.Sqrt( distanceSquared)) );
					}
				}
			}
		}
		return affected.ToArray();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * Obtain the Unity GameObject associated with an LVC Game ID
	 * 
	 * @param lvcID the LVC Game identifier
	 * @return the corresponding Unity GameObject, or null if no such GameObject exists
	 */
	public GameObject GameObjectForLvcId( long lvcID )
	{
		LVCPair<LVCGame.EntityData, GameObject> result = GameObjectAndEntityDataForLvcId( lvcID );
		if( result == null )
			return null;
		return result.B;
	}
	
	/**
	 * Obtain the Unity GameObject associated with an LVC Game ID
	 * 
	 * @param lvcID the LVC Game identifier
	 * @return the corresponding Unity GameObject, or null if no such GameObject exists
	 */
	public LVCPair<LVCGame.EntityData, GameObject> GameObjectAndEntityDataForLvcId( long lvcID )
	{
		LVCPair<LVCGame.EntityData, GameObject> result = null;
		lock( externalEntityRegistry )
		{
			externalEntityRegistry.TryGetValue( lvcID, out result );
		}
		return result;
	}
	
	/**
	 * Obtain the Unity GameObject associated with an LVC Game ID
	 * 
	 * @param gameObject the LVC Game identifier
	 * @return the corresponding LVC Game ID, or null if no such GameObject exists
	 */
	public long LvcIdForGameObject( GameObject gameObject )
	{
		long result = -1;
		lock( gameObjectToLvcIdMap )
		{
			gameObjectToLvcIdMap.TryGetValue( gameObject, out result );
		}
		return result;
	}
	
	/*
	 * TODO
	 */
	private class SimTicker
	{
		private int waitTime;
		private bool shouldStop;
		private ILVCClient lvcClient;
		
		public SimTicker( ref ILVCClient lvcClient, float waitTime )
		{
			this.lvcClient = lvcClient;
			this.waitTime = (int)(waitTime * 1000);
			this.shouldStop = false;
		}
		
		public void shutDown()
		{
			this.shouldStop = true;
		}
		
		public void ThreadProc()
		{
			while( !shouldStop )
			{
				if( this.lvcClient == null )
				{
					shouldStop = true;
				}
				else
				{
					this.lvcClient.Tick( this.waitTime );
					Thread.Sleep(waitTime);
				}
			}
			this.lvcClient = null;
		}
	}
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
	/**
	 * Obtain a reference to the LVC Client instance.
	 * 
	 * @return the LVC Client instance.
	 */
	public static LVCUnityAmbassador GetInstance()
	{
		if(lvcUnityAmbassador == null)
			lvcUnityAmbassador = new LVCUnityAmbassador();
		
		return lvcUnityAmbassador;
	}
}
