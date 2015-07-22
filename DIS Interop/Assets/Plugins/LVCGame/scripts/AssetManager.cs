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

using UnityEngine;
using System.Collections.Generic;

/**
 * This is a MonoBehaviour which provides a way to manage standard prefabs, 
 * materials and other resources which must be instantiated during the 
 * course of an LVC Game.
 * 
 * The primary purpose is map an LVC type name, such as 'm1a1', to the prefab
 * representing that type within Unity. Thus, when LVC detects a new 'm1a1'
 * appearing in the simulation context, we can ask the AssetManager to provide
 * the appropriate prefab for it.
 * 
 * Like the ExternalLVCEntityHelper, this behaviour should be attached to 
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
 * See also: ExternalLVCEntityHelper
 * 
 */
public class AssetManager : MonoBehaviour 
{	
	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	private static Dictionary<string, GameObject> prefabsTracker = new Dictionary<string, GameObject>();
	private static Dictionary<string, Material> materialsTracker = new Dictionary<string, Material>();
	private static Dictionary<string, Object> genericAssetsTracker = new Dictionary<string, Object>();
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------
	public string[] prefabKeys;
	public GameObject[] prefabs;
	
	public string[] materialKeys;
	public Material[] materials;
	
	public string[] genericAssetKeys;
	public Object[] genericAssets;
	
	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	
	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	void OnEnable()
	{
		Initialise();
	}
	
	private void Initialise()
	{
		InitPrefabsTracker();
		InitMaterialsTracker();
		InitGenericAssetsTracker();
		
		// clear the original arrays...?
	}
	
	private void InitPrefabsTracker()
	{
		if( prefabKeys.Length != prefabs.Length )
			Debug.LogError("AssetManager: Keys/Values are of different lengths ("+prefabKeys.Length+" -vs- "+prefabs.Length+") for Prefabs Tracker - initilialisation skipped.");
		else
		{
			for( int i=0; i<prefabKeys.Length; i++ )
			{
				string key = prefabKeys[i];
				GameObject val = prefabs[i];
				if( key!=null && key.Length>0 )
				{
					if( val!=null)
					{
						if( !prefabsTracker.ContainsKey(key) )
							prefabsTracker.Add( key, val );
						else
							Debug.LogWarning("AssetManager: Prefabs Tracker already contained the key '"+key+"' - entry was skipped.");
					}
					else
						Debug.LogWarning("AssetManager: Empty/Null value for key '"+key+"' for Prefabs Tracker - entry was skipped.");
				}
				else
					Debug.LogWarning("AssetManager: Empty/Null key for Prefabs Tracker - entry was ignored.");
			}
		}
	}
	
	private void InitMaterialsTracker()
	{
		if( materialKeys.Length != materials.Length )
			Debug.LogError("AssetManager: Keys/Values are of different lengths ("+prefabKeys.Length+" -vs- "+prefabs.Length+") for Materials Tracker - initilialisation skipped.");
		else
		{
			for( int i=0; i<materialKeys.Length; i++ )
			{
				string key = materialKeys[i];
				Material val = materials[i];
				if( key!=null && key.Length>0 )
				{
					if( val!=null)
					{
						if( !materialsTracker.ContainsKey(key) )
							materialsTracker.Add( key, val );
						else
							Debug.LogWarning("AssetManager: Materials Tracker already contained the key '"+key+"' - entry was skipped.");
					}
					else
						Debug.LogWarning("AssetManager: Empty/Null value for key '"+key+"' for Materials Tracker - entry was skipped.");
				}
				else
					Debug.LogWarning("AssetManager: Empty/Null key for Materials Tracker - entry was ignored.");
			}
		}
	}

	private void InitGenericAssetsTracker()
	{
		if( genericAssetKeys.Length != genericAssets.Length )
			Debug.LogError("AssetManager: Keys/Values are of different lengths ("+genericAssetKeys.Length+" -vs- "+genericAssets.Length+") for Generic Assets Tracker - initilialisation skipped.");
		else
		{
			for( int i=0; i<genericAssetKeys.Length; i++ )
			{
				string key = genericAssetKeys[i];
				Object val = genericAssets[i];
				if( key!=null && key.Length>0 )
				{
					if( val!=null)
					{
						if( !genericAssetsTracker.ContainsKey(key) )
							genericAssetsTracker.Add( key, val );
						else
							Debug.LogWarning("AssetManager: Generic Assets Tracker already contained the key '"+key+"' - entry was skipped.");
					}
					else
						Debug.LogWarning("Empty/Null value for key '"+key+"' for Generic Assets Tracker - entry was skipped.");
				}
				else
					Debug.LogWarning("AssetManager: Empty/Null key for Generic Assets Tracker - entry was ignored.");
			}
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
	public static GameObject GetPrefab( string key )
	{
		GameObject result = null;
		if(!prefabsTracker.TryGetValue(key, out result))
			Debug.LogWarning ("AssetManager: Unable to locate prefab for '"+key+"'");
		return result;
	}
	
	public static Material GetMaterial( string key )
	{
		Material result = null;
		if(!materialsTracker.TryGetValue(key, out result))
			Debug.LogWarning ("AssetManager: Unable to locate material for '"+key+"'");
		return result;
	}
	
	public static Object GetGenericAsset( string key )
	{
		Object result = null;
		if(!genericAssetsTracker.TryGetValue(key, out result))
			Debug.LogWarning ("AssetManager: Unable to locate generic asset for '"+key+"'");
		return result;
	}
}
