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
using System.Collections;

/**
 * An abstract class defining the interface which must be realised for
 * an LVCExtraUpdateHandler.
 * 
 * A realised LVCExtraUpdateHandler provides handling specific to an
 * entity type for LVC update events. 
 * 
 * For example, an LVCExtraUpdateHandler could be used to inspect 
 * articulation data and rotate the turrent of an M1A1 tank 
 * representation, or emit a dust trail if the tank is moving.
 */
public abstract class LVCExtraUpdateHandler : MonoBehaviour
{
	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------

	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	
	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	/**
	 * Handle update event data
	 * 
	 * @param entityData the LVC update data information
	 * @param gO the Unity GameObject representation of the updated entity
	 */	
	public abstract void HandleExtra( LVCGame.EntityData entityData, GameObject gO );
	
	/**
	 * Handle update event data
	 * 
	 * @param entityDataManager the entityDataManager containing entity state information
	 * @param gO the Unity GameObject representation of the updated entity
	 */	
	public abstract void HandleExtra( EntityDataManager entityDataManager, GameObject gO );
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
}
