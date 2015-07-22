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
 * This class is intended as a centralised "repository" of LVC Game type names 
 * and should match up with:
 * 
 *    - the names used for DIS enumerations in the "lvc_to_Unity.config" file
 *    - the key values populated in the AssetManager
 * 
 * The main aim is to avoid hard coded strings appearing everywhere, requiring 
 * a change in a name to be corrected everywhere it occurs. By using the static
 * string constants defined in this class, a name change only needs to be
 * corrected in this class.
 * 
 * There is no necessity to use this class, but generally speaking it tends to 
 * keep the code cleaner and easier to maintain.
 */
public class LVCGameTypes
{
	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	public static readonly string EXAMPLE="example";
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------

	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	private LVCGameTypes(){}
	
	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////
	
	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
}
