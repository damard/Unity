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
 * an LVCExtraFireHandler.
 * 
 * A realised LVCExtraFireHandler provides handling specific to an
 * entity type for LVC fire events. 
 * 
 * For example, an LVCExtraFireHandler could be used to create a muzzle
 * flash from the main barrel of an M1A1 tank representation, or emit
 * shell casings from the secondary machine gun.
 */
public abstract class LVCExtraFireHandler : MonoBehaviour
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
	 * Handle fire weapon event data
	 * 
	 * @param fireWeaponData the LVC weapon fire data information
	 * @param gO the Unity GameObject representation of the firing entity
	 */
	public abstract void HandleFire( LVCGame.FireWeaponData fireWeaponData, GameObject gO );
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
}
