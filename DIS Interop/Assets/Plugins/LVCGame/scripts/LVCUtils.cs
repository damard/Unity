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
using System.Collections.Generic;
using System.Text.RegularExpressions;

/**
 * A utility class to provide access to common functions used when bridging the Unity to
 * LVC spaces, such as conversion of angles from degrees to radians, reading configuration
 * files and so on.
 */
public class LVCUtils 
{
	//----------------------------------------------------------
	//                   ENUMERATIONS
	//----------------------------------------------------------
	public enum FORCE{OTHER=0, FRIENDLY=1, OPPOSING=2, NEUTRAL=3};
	public enum DIS_FIELD_INDEX{KIND=0,DOMAIN=1,COUNTRY=2,CATEGORY=3,SUBCATEGORY=4,SPECIFIC=5,OTHER=6};
	public enum DIS_KIND{OTHER=0,PLATFORM=1,MUNITION=2,LIFE_FORM=3,ENVIRONMENTAL=4,CULTURAL_FEATURE=5,SUPPLY=6,RADIO=7,EXPENDABLE=8,SENSOR_EMITTER=9};
	public enum DIS_DOMAIN{OTHER=0,LAND=1,AIR=2,SURFACE=3,SUBSURFACE=4,SPACE=5};
	public enum DIS_MUNITION_DOMAIN{OTHER=0,ANTI_AIR=1,ANTI_ARMOR=2,ANTI_GUIDED_WEAPON=3,ANTIRADAR=4,ANTISATELLITE=5,ANTISHIP=6,ANTISUBMARINE=7,ANTIPERSONNEL=8,BATTLEFIELD_SUPPORT=9,STRATEGIC=10,TACTICAL=11,DIRECTED_ENERGY_DE_WEAPON=12};

	//----------------------------------------------------------
	//                   STATIC VARIABLES
	//----------------------------------------------------------
	// this is name of the LVC Client type, in this case Unity
	public static readonly string CLIENT_TYPE = "Unity";
	
	// strings which equate to a boolean "true" value (lower case)
	private static readonly string[] TRUE_BOOLEANS = {"true","on","1","yes"};
	
	// this is the directory under the Unity "Resources" folder containing the LVCGame config file(s)
	private static readonly string LVCGAME_RESOURCE_DIR = ".LVCGame";
	// this is the directory under the Unity "Resources" folder containing the LVCGame config file(s)
	private static readonly string LVCGAME_CONFIG_COMMENT_PREFIX = "#";
	// used for converting the degree angles used in Unity to the radian angles used in LVC Game, and vice versa
	private static readonly double DEGREES_TO_RADIANS = ( Mathf.PI / 180.0 );
	// the OS specific folder separator for the current OS
	private static readonly string LOCALIZED_FOLDER_SEP = System.IO.Path.DirectorySeparatorChar.ToString();
	
	private static Dictionary<string, Dictionary<string, string>> CONFIGURATION_MAP = new Dictionary<string, Dictionary<string, string>>();
	
	// Important configuration filenames and keys
	private static readonly string CONF_UNITYCLIENT_FILENAME = LVCUtils.CLIENT_TYPE+"Client.config";
	private static readonly string CONF_OriginLatitude   = "OriginLatitude";
	private static readonly string CONF_OriginLongitude  = "OriginLongitude";
	private static readonly string CONF_OriginAltitude   = "OriginAltitude";
	private static readonly string CONF_OriginEasting    = "OriginEasting";
	private static readonly string CONF_OriginNorthing   = "OriginNorthing";
	private static readonly string CONF_OriginZone       = "OriginZone";
	private static readonly string CONF_OriginHemisphere = "OriginHemisphere";
	
	// a regular expression used to extract values from -93*23'52.60" style 
	// formatted lat/long values found in some configuration files
	private static readonly string CONF_DMS_REGEX = @"(\-?)(\d+)\*(\d+)\'(\d+(\.\d+)?)";
	
	private static LVCGame.Vector3 lvcWorldOriginLLA;
	private static LVCGame.UTMCoordinate lvcWorldOriginUTM;
	private static LVCGame.Vector3 v3zero = new LVCGame.Vector3(0.0, 0.0, 0.0);
	private static bool isLvcWorldOriginInitialised = false;
	
	//----------------------------------------------------------
	//                   INSTANCE VARIABLES
	//----------------------------------------------------------

	//----------------------------------------------------------
	//                      CONSTRUCTORS
	//----------------------------------------------------------
	
	//----------------------------------------------------------
	//                    INSTANCE METHODS
	//----------------------------------------------------------
	
	////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////// Accessor and Mutator Methods ///////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////

	//----------------------------------------------------------
	//                     STATIC METHODS
	//----------------------------------------------------------
	/**
	 * Convenience method to constrain a vlue to be within certain bounds.
	 * 
	 * @param valueToConstrain the value to be constrained.
	 * @param min the minimum value which may be returned
	 * @param max the maximum value which may be returned
	 * @return the value constrained to fall within the min and max (inclusive).
	 */
	public static float ConstrainValue( float valueToConstrain, float min, float max)
	{
		return Mathf.Max(min, Mathf.Min (max, valueToConstrain));
	}
	
	/**
	 * Convenience method to convert a string to a boolean value.
	 * 
	 * @param val the string representation of the boolean value.
	 * @return the true of the string is one of TRUE_BOOLEANS, false otherwise.
	 */
	public static bool StringToBool( string val )
	{
		string tempLower = val.Trim().ToLower();
		foreach(string s in TRUE_BOOLEANS)
		{
			if( s == tempLower )
				return true;
		}
		return false;
	}
	
	/**
	 * Convenience method to convert a string to an integer value.
	 * 
	 * @param val the string representation of the integer value.
	 * @param fallback the value to return in the case that the string cannot be interpreted 
	 *        as an integer.
	 * @return the integer value of the string, or the fallback value in the case that the string 
	 *        cannot be interpreted as such.
	 */
	public static int StringToInt( string val, int fallback )
	{
        try
        {
            return System.Convert.ToInt32( val.Trim() );
        }
        catch ( System.Exception )
        {
			return fallback;
        }
	}
	
	/**
	 * Convenience method to convert a string to a float value.
	 * 
	 * @param val the string representation of the float value.
	 * @param fallback the value to return in the case that the string cannot be interpreted 
	 *        as a float.
	 * @return the float value of the string, or the fallback value in the case that the string 
	 *        cannot be interpreted as such.
	 */
	public static float StringToFloat( string val, float fallback )
	{
        try
        {
            return System.Convert.ToSingle( val.Trim() );
        }
        catch ( System.Exception )
        {
			return fallback;
        }
	}
	
	/**
	 * Convenience method to convert a string to a double value.
	 * 
	 * @param val the string representation of the double value.
	 * @param fallback the value to return in the case that the string cannot be interpreted 
	 *        as a double.
	 * @return the double value of the string, or the fallback value in the case that the string 
	 *        cannot be interpreted as such.
	 */
	public static double StringToDouble( string val, double fallback )
	{
        try
        {
            return System.Convert.ToDouble( val.Trim() );
        }
        catch ( System.Exception )
        {
			return fallback;
        }
	}
	
	/**
	 * Convenience method to convert a string to a colour, interpreting the string as a hex 
	 * RGB representation. Accepts all of the following color notations:
	 *  RGB      - e.g. F0F      (magenta)
	 *  RGBA     - e.g. F0F8     (magenta, 50% alpha)
	 *  RRGGBB   - e.g. FF00FF   (magenta)
	 *  RRGGBBAA - e.g. FF00FF88 (magenta, 50% alpha)
	 * 
	 * @param hex the string representation of the colour's RGB (or RGBA) values.
	 * @return the colour, or black in the case that the string cannot be interpreted as such.
	 */
	public static Color hexToColor( string hex )
	{
		float[] rgba = {0.0F, 0.0F, 0.0F, 1.0F};
		
		if( hex.Length == 3 || hex.Length == 4)
		{
			// RGB or RGBA
			for(int i=0; i<hex.Length; i++)
			{
				try
				{
					string digit = hex.Substring(i,1);
					int val = System.Convert.ToInt32( digit+digit, 16 );
					rgba[i] = val/255.0F;
				}
				catch( System.OverflowException )
				{
					// ignore
				}
			}
		}
		else if(hex.Length == 6 || hex.Length == 8)
		{
			for(int i=0; i<hex.Length; i+=2)
			{
				try
				{
					int val = System.Convert.ToInt32( hex.Substring(i,2), 16 );
					rgba[i] = val/255.0F;
				}
				catch( System.OverflowException )
				{
					// ignore
				}
			}
		}
		
		return new Color(rgba[0],rgba[1],rgba[2],rgba[3]);
	}
	
	/**
	 * Convert an angle expressed in degrees to the equivalent value in radians
	 * 
	 * @param degrees the angle expressed in degrees
	 * @return the angle expressed in radians
	 */
	public static double Degrees2Radians ( double degrees ) 
	{
		return degrees * DEGREES_TO_RADIANS;
	}
	
	/**
	 * Convert an angle expressed in radians to the equivalent value in degrees
	 * 
	 * @param radians the angle expressed in radians
	 * @return the angle expressed in degrees
	 */
	public static double Radians2Degrees ( double radians ) 
	{
		return radians / DEGREES_TO_RADIANS;
	}
	
	/**
	 * A utility method to convert a degrees, minutes, seconds representation of a latitude or 
	 * longitude to a value in decimal degrees. The expected format is:
	 *     d*m's"
	 * ...with the following being two valid examples:
	 *     -93*23'52.60"
	 *     30*53'04.09"
	 * 
	 * @param dms the degrees, minutes, seconds representation of a latitude or longitude
	 * @return the decimal degree equivalent
	 */
	public static double DMStoDegrees( string dms )
	{
		double result = 0.0;
		// attempt to parse the d*m's" style notation
		string[] dmsParts = Regex.Split( dms, CONF_DMS_REGEX );
		try
		{
			result =  System.Convert.ToDouble( dmsParts[2] );
			result += System.Convert.ToDouble( dmsParts[3] ) / 60.0;
			result += System.Convert.ToDouble( dmsParts[4] ) / 3600.0;
			// correct for sign (-ve/+ve)
			result *= dmsParts[1]=="-"?-1:1;
		}
		catch( System.Exception e )
		{
			// if anything at all goes wrong, we just log it and move on
			Debug.LogError( "Failed to parse '"+dms+"' as d*m's\" notation: "+e.Message );
		}
		return result;
	}
	
	/**
	 * A utility method to convert a degrees, minutes, seconds representation of a latitude or 
	 * longitude to a value in radians. The expected format is:
	 *     d*m's"
	 * ...with the following being two valid examples:
	 *     -93*23'52.60"
	 *     30*53'04.09"
	 * 
	 * @param dms the degrees, minutes, seconds representation of a latitude or longitude
	 * @return the radian equivalent
	 */
	public static double DMStoRadians( string dms )
	{
		return Degrees2Radians( DMStoDegrees(dms) );
	}
	
	/**
	 * A utility method to convert a decimal degrees latitude or longitude value to a 
	 * degrees, minutes, seconds representation. The output format is:
	 *     d*m's"
	 * ...with the following being two examples:
	 *     -93*23'52.60"
	 *     30*53'04.09"
	 * 
	 * @param degrees the latitude or longitude value
	 * @param accuracy (optional) the number of decimal places accuracy required for the 
	 *        seconds component
	 * @return the equivalent degrees, minutes, seconds representation
	 */
	public static string DegreesToDMS( double degrees, int accuracy )
	{
		bool isNeg = degrees < 0.0;
		double absDegValue =Mathf.Abs((float)degrees);
		int deg = (int)( absDegValue );
		double min = (int)( (absDegValue - deg) * 60 );
		double sec = ( absDegValue - deg - (min/60.0) ) * 3600;
		
		return (isNeg?"-":"")+deg.ToString()+"\u00B0"+min.ToString()+"\u2032"+sec.ToString("f"+accuracy.ToString())+"\u2033";
	}
	
	/**
	 * A utility method to convert a radian latitude or longitude value to a 
	 * degrees, minutes, seconds representation. The output format is:
	 *     d*m's"
	 * ...with the following being two examples:
	 *     -93*23'52.60"
	 *     30*53'04.09"
	 * 
	 * @param radians the latitude or longitude value
	 * @param accuracy (optional) the number of decimal places accuracy required for the 
	 *        seconds component
	 * @return the equivalent degrees, minutes, seconds representation
	 */
	public static string RadiansToDMS( double radians, int accuracy )
	{
		return DegreesToDMS( Radians2Degrees(radians), accuracy );
	}
	
	/**
	 * A utility method to convert a latitude/longitude/altitude position representaion
	 * to a local tangent plane representation. Uses the world origin obtained from the
	 * LVC configuration as the primary reference point for the conversion.
	 * 
	 * @param lla the latitude/longitude/altitude position to convert. latitude and longitude 
	 *        are expected to be in radians, altitude is in meters.
	 * @return the local tangent plane coordinates in meters. The axes are as follows:
	 *        AXIS (+ve): | LVC
	 *        ------------+-----
	 *        NORTH       |  X
	 *        EAST        |  Y
	 *        UP          |  Z
	 */
	public static LVCGame.Vector3 llaToLtp( LVCGame.Vector3 lla )
	{
		return LVCGame.LVCCoordinates.llaToLtp( ref v3zero, ref lla );
	}

	/**
	 * A utility method to convert a local tangent plane position representaion to a 
	 * latitude/longitude/altitude representation. Uses the world origin obtained from the
	 * LVC configuration as the primary reference point for the conversion.
	 * 
	 * @param the local tangent plane coordinates in meters. The axes are as follows:
	 *        AXIS (+ve): | LVC
	 *        ------------+-----
	 *        NORTH       |  X
	 *        EAST        |  Y
	 *        UP          |  Z
	 * @return lla the latitude/longitude/altitude position to convert. latitude and longitude 
	 *         are in radians, altitude is in meters.
	 */
	public static LVCGame.Vector3 ltpToLla( LVCGame.Vector3 ltp )
	{
		return LVCGame.LVCCoordinates.ltpToLla( ref v3zero, ref ltp );
	}

	/**
	 * Convert a Unity Engine Vector3 into an LVC Game Vector 3, correcting for the swapping
	 * of the Y and Z axes between the coordinate systems
	 *        AXIS (+ve): | UNITY | LVC  
	 *        ------------+-------+-----
	 *        NORTH       |  X    |  X
	 *        EAST        |  Z    |  Y
	 *        UP          |  Y    |  Z
	 * 
	 * @param UEv3 the Unity Engine Vector3 to convert
	 * @return the equivalent LVC Game Vector3, with corrected axes (Y and Z axes swapped)
	 */
	public static LVCGame.Vector3 UnityCoords_to_LVCGameCoords( UnityEngine.Vector3 UEv3 )
	{
		return new LVCGame.Vector3( UEv3.x, UEv3.z, UEv3.y );
		//                                  ^===SWAPPED==^
	}
	
	/**
	 * Convert an LVC Game Vector3 into a Unity Engine Vector 3, correcting for the swapping
	 * of the Y and Z axes between the coordinate systems
	 *        AXIS (+ve): | LVC | UNITY
	 *        ------------+-----+-------
	 *        NORTH       |  X  |   X
	 *        EAST        |  Y  |   Z
	 *        UP          |  Z  |   Y
	 * 
	 * @param LVCv3 the LVC Game Vector3 to convert
	 * @return the equivalent Unity Engine Vector3, with corrected axes (Y and Z axes swapped)
	 */
	public static UnityEngine.Vector3 LVCGameCoords_to_UnityCoords( LVCGame.Vector3 LVCv3 )
	{
		return new UnityEngine.Vector3( (float)LVCv3.x, (float)LVCv3.z, (float)LVCv3.y );
		//                                                           ^====SWAPPED====^
	}
	
	/**
	 * Convert a Unity Engine Quaternion orientation into an LVC Game Vector 3, correcting for 
	 * the different interpretations of rotation axes between the systems
	 *       AXIS (+ve): | UNITY | LVC | 
	 *       ------------+-------+-----
	 *       ROLL        |   X   | -Z
	 *       YAW         |   Y   |  X
	 *       PITCH       |   Z   |  Y
	 * 
	 * @param UEv3 the Unity Engine Vector3 to convert
	 * @return the equivalent LVC Game Vector3, with corrected axes
	 */
	public static LVCGame.Vector3 UnityOrientation_to_LVCGameOrientation( UnityEngine.Quaternion orientation )
	{
		UnityEngine.Vector3 eulerAngles = orientation.eulerAngles;
		LVCGame.Vector3 result = new LVCGame.Vector3(
			 LVCUtils.Degrees2Radians( eulerAngles.y ), // YAW
			 LVCUtils.Degrees2Radians( eulerAngles.z ), // PITCH
			-LVCUtils.Degrees2Radians( eulerAngles.x )  // ROLL
		);
		return result;
	}
	
	/**
	 * Convert a LVC Game Vector 3 orientation into a Unity Engine Quaternion, correcting for 
	 * the different interpretations of rotation axes between the systems
	 *       AXIS (+ve): | LVC | UNITY
	 *       ------------+-----+-------
	 *       YAW         |  X  |   Y
	 *       PITCH       |  Y  |   Z
	 *       ROLL        |  Z  |  -X
	 * 
	 * @param orientation the LVC Game Vector3 to convert
	 * @return the equivalent Unity Engine Quaternion, with corrected axes
	 */
	public static UnityEngine.Quaternion LVCGameOrientation_to_UnityOrientation( LVCGame.Vector3 orientation )
	{
		float yaw   =  (float)orientation.x;
		float pitch =  (float)orientation.y;
		float roll  = -(float)orientation.z;
		
		float cY = Mathf.Cos(yaw / 2);
		float cP = Mathf.Cos(pitch / 2);
		float cR = Mathf.Cos(roll / 2);
		float sY = Mathf.Sin(yaw / 2);
		float sP = Mathf.Sin(pitch / 2);
		float sR = Mathf.Sin(roll / 2);
		
		float w = (cY*cP*cR) - (sY*sP*sR);
		float x = (sY*sP*cR) + (cY*cP*sR);
		float y = (sY*cP*cR) + (cY*sP*sR);
		float z = (cY*sP*cR) - (sY*cP*sR);
		
		UnityEngine.Quaternion result = new UnityEngine.Quaternion();
		result.w = w;
		result.x = z;
		result.y = y;
		result.z = x;		
		
		return result;
	}
	
	/**
	 * A utility function to transfer the values from a UnityEngine Vector3 into an LVC Vector3.
	 * 
	 * @param source the UnityEngine.Vector3 instance which is the source of the values
	 * @param target the LVCGame.Vector3 instance which is to receive the values from the source
	 */
	public static void TransferValues( UnityEngine.Vector3 source, ref LVCGame.Vector3 target )
	{
		target.x = source.x;
		target.y = source.y;
		target.z = source.z;
	}
	
	/**
	 * A utility function to transfer the values from an LVC Vector3 into a UnityEngine Vector3.
	 * 
	 * @param source the LVCGame.Vector3 instance which is the source of values from the source
	 * @param target the UnityEngine.Vector3 instance which is to receive the values
	 */
	public static void TransferValues( LVCGame.Vector3 source, ref UnityEngine.Vector3 target )
	{
		target.x = (float)source.x;
		target.y = (float)source.y;
		target.z = (float)source.z;
	}
	
	/**
	 * A utility function to transfer the values from one LVC Vector3 into another LVC Vector3.
	 * 
	 * @param source the LVCGame.Vector3 instance which is the source of the values from the source
	 * @param target the LVCGame.Vector3 instance which is to receive the values
	 */
	public static void TransferValues( LVCGame.Vector3 source, ref LVCGame.Vector3 target )
	{
		target.x = source.x;
		target.y = source.y;
		target.z = source.z;
	}
	
	/**
	 * A utility function to transfer the values from an UnityEngine Vector3 into another 
	 * UnityEngine Vector3.
	 * 
	 * @param source the LVCGame.Vector3 instance which is the source of values from the source
	 * @param target the UnityEngine.Vector3 instance which is to receive the values
	 */
	public static void TransferValues( UnityEngine.Vector3 source, ref UnityEngine.Vector3 target )
	{
		target.x = source.x;
		target.y = source.y;
		target.z = source.z;
	}
	
	/**
	 * A utility function to transfer the values from one Quarternion into another Quaternion.
	 * 
	 * @param source the Quaternion instance which is the source of the values
	 * @param target the Quaternion instance which is to receive the values from the source
	 */
	public static void TransferValues( Quaternion source, ref Quaternion target )
	{
		target.w = source.w;
		target.x = source.x;
		target.y = source.y;
		target.z = source.z;
	}
	
	/**
	 * Returns the root directory under which LVC Game configuration files are located.
	 * 
	 * To fit in with the Unity Game Engine framework, this uses Application.dataPath, and so will be...
	 *    Unity Editor  :	<path tp project folder>/Assets/LVCGAME_RESOURCE_DIR
	 *    Mac player    :	<path to player app bundle>/Contents/LVCGAME_RESOURCE_DIR
	 *    iPhone player :	<path to player app bundle>/<AppName.app>/Data/LVCGAME_RESOURCE_DIR
	 *    Win player    :	<path to executablename_Data folder>/LVCGAME_RESOURCE_DIR
	 *    Web player    :	The absolute url to the player data file folder (without the actual data file name)/LVCGAME_RESOURCE_DIR
	 *    Flash         :	The absolute url to the player data file folder (without the actual data file name)/LVCGAME_RESOURCE_DIR
	 * Note that the string returned on a PC will use a forward slash as a folder separator.
	 * 
	 * Note also that there is no training folder separator slash after the directory name returned.
	 */
    public static string GetLVCConfigPath () 
	{
		return Application.dataPath + "/" + LVCGAME_RESOURCE_DIR;
	}
	
	/**
	 * Utility method to ensure that a path uses the OS sepcific path separator character, 
	 * rather than the generic forward slash favoured by Unity. This is primarily to ensure
	 * that on Windows platforms we can locate files correctly
	 * 
	 * @param path the path string which should be localized
	 * @return the localized path string
	 */
    public static string LocalizePath ( string path ) 
	{
		return path.Replace( "/", LOCALIZED_FOLDER_SEP );
	}
	
	/**
	 * Obtains a value from an LVC Game configuration file based on the provided key. 
	 * If no such key was found in the configuration file, the fallback value is returned.
	 * 
	 * Note that configuration file contents are cached, so that only the first call
	 * to retrieve a value from a given configuration file results in a file being 
	 * read - subsequent calls to retrieve a value from the same file use the cached
	 * contents for the lookup.
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @param fallback (optional) the value to return if the key is not found
	 * @return the value for the key in the provided configuration file, or the 
	 *         fallback value if no such key was found.
	 */
	public static string GetLVCConfigValue( string lvcConfigFileName, string key, string fallback )
	{
		Dictionary<string, string> configuration = null;
		if( !CONFIGURATION_MAP.TryGetValue( lvcConfigFileName, out configuration ) )
		{
			// we don't have this configuration file cached yet - read it in now
			configuration = ReadLVCConfig( lvcConfigFileName );
			CONFIGURATION_MAP.Add (lvcConfigFileName, configuration);
		}
		
		configuration.TryGetValue( key, out fallback );
		return fallback;
	}
	
	/**
	 * Determine if an LVC Game configuration file contains a value for the provided key. 
	 * 
	 * Note that this is essentially convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @return true if a value for the key is in the provided configuration file, false otherwise
	 */
	public static bool HasLVCConfigValue( string lvcConfigFileName, string key )
	{
		return ( GetLVCConfigValue(lvcConfigFileName, key, null) != null );
	}
	
	/**
	 * Determine if an LVC Game configuration file contains values for *all* of the 
	 * provided keys. 
	 * 
	 * Note that this is essentially convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @return true if all of the given keys have configured values in the provided 
	 *         configuration file, false otherwise
	 */
	public static bool HasAllLVCConfigValues( string lvcConfigFileName, string[] keys )
	{
		foreach( string key in keys )
		{
			if( GetLVCConfigValue(lvcConfigFileName, key, null) == null )
				return false;
		}
		return true;
	}
	
	/**
	 * Determine if an LVC Game configuration file contains values for *any* of the 
	 * provided keys. 
	 * 
	 * Note that this is essentially convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @return true if any of the given keys have configured values in the provided 
	 *         configuration file, false otherwise
	 */
	public static bool HasAnyLVCConfigValues( string lvcConfigFileName, string[] keys )
	{
		foreach( string key in keys )
		{
			if( GetLVCConfigValue(lvcConfigFileName, key, null) != null )
				return true;
		}
		return false;
	}
	
	/**
	 * Convenience method to obtains a boolean value from an LVC Game configuration 
	 * file based on the provided key. If no such key was found in the configuration 
	 * file, the fallback value is returned.
	 * 
	 * Note that this is a convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @param fallback (optional) the value to return if the key is not found
	 * @return the value for the key in the provided configuration file, or the 
	 *         fallback value if no such key was found.
	 */
	public static bool GetBoolLVCConfigValue( string lvcConfigFileName, string key, bool fallback )
	{
		string result = GetLVCConfigValue( lvcConfigFileName, key, null );
		
		if( result == null)
			return fallback;
		
		return StringToBool( result );
	}
	
	/**
	 * Convenience method to obtains an integer value from an LVC Game configuration 
	 * file based on the provided key. If no such key was found in the configuration 
	 * file, the fallback value is returned.
	 * 
	 * Note that this is a convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @param fallback (optional) the value to return if the key is not found
	 * @return the value for the key in the provided configuration file, or the 
	 *         fallback value if no such key was found.
	 */
	public static int GetIntLVCConfigValue( string lvcConfigFileName, string key, int fallback )
	{
		string result = GetLVCConfigValue( lvcConfigFileName, key, null ).ToLower();
		
		if( result == null )
			return fallback;
		
        try
        {
            return System.Convert.ToInt32( result );
        }
        catch ( System.Exception )
        {
			// ignore
        }
		return fallback;
	}
	
	/**
	 * Convenience method to obtains an integer value from an LVC Game configuration 
	 * file based on the provided key. If no such key was found in the configuration 
	 * file, the fallback value is returned.
	 * 
	 * Note that this is a convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @param fallback (optional) the value to return if the key is not found
	 * @return the value for the key in the provided configuration file, or the 
	 *         fallback value if no such key was found.
	 */
	public static long GetLongLVCConfigValue( string lvcConfigFileName, string key, long fallback )
	{
		string result = GetLVCConfigValue( lvcConfigFileName, key, null ).ToLower();
		
		if( result == null )
			return fallback;
		
        try
        {
            return System.Convert.ToInt64( result );
        }
        catch ( System.Exception )
        {
			// ignore
        }
		return fallback;
	}
	
	/**
	 * Convenience method to obtains a float value from an LVC Game configuration 
	 * file based on the provided key. If no such key was found in the configuration 
	 * file, the fallback value is returned.
	 * 
	 * Note that this is a convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @param fallback (optional) the value to return if the key is not found
	 * @return the value for the key in the provided configuration file, or the 
	 *         fallback value if no such key was found.
	 */
	public static float GetFloatLVCConfigValue( string lvcConfigFileName, string key, float fallback )
	{
		string result = GetLVCConfigValue( lvcConfigFileName, key, null ).ToLower();
		
		if( result == null )
			return fallback;
		
        try
        {
            return System.Convert.ToSingle( result );
        }
        catch ( System.Exception )
        {
			// ignore
        }
		return fallback;
	}
	
	/**
	 * Convenience method to obtains a float value from an LVC Game configuration 
	 * file based on the provided key. If no such key was found in the configuration 
	 * file, the fallback value is returned.
	 * 
	 * Note that this is a convenience wrapper around GetLVCConfigValue()
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @param key the name of the parameter to find the value for (case sensitive).
	 * @param fallback (optional) the value to return if the key is not found
	 * @return the value for the key in the provided configuration file, or the 
	 *         fallback value if no such key was found.
	 */
	public static double GetDoubleLVCConfigValue( string lvcConfigFileName, string key, double fallback )
	{
		//Debug.Log("Filename :" + lvcConfigFileName);
		//Debug.Log("Key: " + key);
		string result = GetLVCConfigValue( lvcConfigFileName, key, null ).ToLower();
		//Debug.Log(result);
		if( result == null )
			return fallback;
		
        try
        {
            return System.Convert.ToDouble( result );
        }
        catch ( System.FormatException )
        {
			// ignore
        }
		return fallback;
	}
	
	/**
	 * Reads an LVC Game configuration file, and returns the contents as an dictionary, 
	 * each entry in the dictionary being a key/value pair from the file (excluding 
	 * all commented and empty lines).
	 * 
	 * @param lvcConfigFileName the name of the file, located in the path returned by 
	 *        GetLVCConfigPath(), or a subfolder thereof. If subfolders are used,
	 *        the forward slash must be used as a folder separator.
	 * @return the key/value pairs in the file as a Dictionary (of strings), excluding 
	 *         any commented and empty lines.
	 */
    public static Dictionary<string, string> ReadLVCConfig( string lvcConfigFileName ) 
	{
		// create a dictionary to hold config values
		Dictionary<string, string> result = new Dictionary<string, string>();
		
		// construct the full path to the configuration file
		string fullPath = GetLVCConfigPath() + "/" + lvcConfigFileName;
		
		// check if the file exists before we begin
		if( !System.IO.File.Exists(fullPath) )
		{
            Debug.LogError("[LVCGAME] The LVC Game configuration file '"+fullPath+"' was not found.");
			return result;
		}
		
		System.IO.StreamReader reader = new System.IO.StreamReader( fullPath );
		int lineNumber = 0;
		string line;
        try   
        {    
			// process the file contents
	        while( (line = reader.ReadLine()) != null ) 
			{
				lineNumber++;
				
				// remove leading and training spaces
				line = line.Trim();
				
				// check if the line is a comment, and skip if so
	            if ( line.StartsWith(LVCGAME_CONFIG_COMMENT_PREFIX) ) 
	                continue;
				
				// check the line isn't empty...
				if ( line.Length > 0 )
				{
					// try to split it on the '=' character
					string[] keyValPair = line.Split('=');
					if(keyValPair.Length==2)
					{
						// if we have a key and a value, add it to the dictionary of config values
	                	result.Add( keyValPair[0].Trim(), keyValPair[1].Trim() );
					}
				}
	        }
        }      
        catch( System.Exception e )
        { 
            Debug.LogError( "[LVCGAME] There was a problem reading the LVC Game configuration file '"+fullPath+"' at line "+lineNumber+":" + e.Message.ToString() );
			return result;
		}
        finally
        {
            reader.Close();
        }
		
		return result;
    }
	
	/**
	 * A utility function which returns the world origin based on values in
	 * the LVC configuration file. Defaults to (0,0,0) if no values are set in
	 * the configuration.
	 */
	public static LVCGame.Vector3 GetLVCWorldOriginLLA()
	{
		// have we already done this?
		if( isLvcWorldOriginInitialised )
			return lvcWorldOriginLLA;
		
		InitializeLVCWorldOrigin();
		return lvcWorldOriginLLA;
	}
	
	/**
	 * A utility function which returns the world origin based on values in
	 * the LVC configuration file. Defaults to (0,0,0) if no values are set in
	 * the configuration.
	 */
	public static LVCGame.UTMCoordinate GetLVCWorldOriginUTM()
	{
		// have we already done this?
		if( isLvcWorldOriginInitialised )
			return lvcWorldOriginUTM;
		
		InitializeLVCWorldOrigin();
		return lvcWorldOriginUTM;
	}
	
	/**
	 * A utility function which returns the world origin based on values in
	 * the LVC configuration file. Defaults to (0,0,0) if no values are set in
	 * the configuration.
	 */
	private static void InitializeLVCWorldOrigin()
	{
		// have we already done this?
		if( isLvcWorldOriginInitialised )
			// no need to go any further
			return;
		
		// check for values specified in configuration file
		if( LVCUtils.HasAllLVCConfigValues( CONF_UNITYCLIENT_FILENAME, 
										 new string[]{CONF_OriginLatitude, CONF_OriginLongitude}) )
		{
			// user has specified the world origin using latitude, longitude and altitude
			// check if they have used the 93*23'52.60" style format for degress minutes and seconds, 
			// or specified  it as a decimal value in radians...
			double latitude = 0.0;
			string latitudeStr = GetLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginLatitude, "0.0" );
			double longitude = 0.0;
			string longitudeStr = GetLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginLongitude, "0.0" );
			try
			{
				// assume latitude is in raw radians to begin with
				latitude = System.Convert.ToDouble( latitudeStr );
			}
			catch(System.FormatException)
			{
				// fall back to d*m's" notation for latitude
				latitude = Degrees2Radians( DMStoDegrees( latitudeStr) );
			}
			try
			{
				// assume longitude is in raw radians to begin with
				longitude = System.Convert.ToDouble( longitudeStr );
			}
			catch(System.FormatException)
			{
				// fall back to d*m's" notation for longitude
				longitude = Degrees2Radians( DMStoDegrees( longitudeStr) );
			}
			
			// altitude is simple as it is in meters - just get the value
			double altitude = GetDoubleLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginAltitude, 0.0 );

			// initialise the world origin vector
			lvcWorldOriginLLA = new LVCGame.Vector3( latitude, longitude, altitude );
			lvcWorldOriginUTM = LVCGame.LVCCoordinates.llaToUtm( ref lvcWorldOriginLLA );
		}
		else if( HasAllLVCConfigValues(CONF_UNITYCLIENT_FILENAME, 
											 new string[]{CONF_OriginEasting, CONF_OriginNorthing, CONF_OriginHemisphere, CONF_OriginZone}) )
		{
			// user has specified the world origin using UTM (Easting, Northing, Hemisphere and Zone)
			double easting = GetDoubleLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginEasting, 0.0 );
			double northing = GetDoubleLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginNorthing, 0.0 );
			double altitude = GetDoubleLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginAltitude, 0.0 );
			char hemisphere = GetLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginHemisphere, "N" )[0];
			int zone = GetIntLVCConfigValue( CONF_UNITYCLIENT_FILENAME, CONF_OriginZone, 0 );
			
			// initialise the world origin from the extracted UTM values
			lvcWorldOriginUTM = new LVCGame.UTMCoordinate( easting, northing, altitude, zone, hemisphere );
			lvcWorldOriginLLA = LVCGame.LVCCoordinates.utmToLla( ref lvcWorldOriginUTM );
		}
		else
		{
			// assume that the world origin is at lat/long (0*0'0", 0*0'0")
			lvcWorldOriginUTM = new LVCGame.UTMCoordinate( 166021, 0, 0, 31, 'N' );
			lvcWorldOriginLLA = new LVCGame.Vector3( 0.0, 0.0, 0.0 );
		}
		
		Debug.Log("World origin is at LLA ("+LVCUtils.RadiansToDMS(lvcWorldOriginLLA.x, 2)+", "
		                                    +LVCUtils.RadiansToDMS(lvcWorldOriginLLA.y, 2)+", "
		                                    +lvcWorldOriginLLA.z+"m)"
		                                    +", UTM ("+DisplayFormatted(lvcWorldOriginUTM)+")" );
		
		// ...and we're done.
		isLvcWorldOriginInitialised = true;
	}
	
	/**
	 * A utility function to create a string using an LVC Vector3 to display the values of the 3 axes 
	 * as metres per second (velocity). This is used for logging purposes.
	 * 
	 * @param v3 the LVC Vector3 containing the velocity values for each axis
	 * @return the formatted string
	 */
	public static string DisplayAsMs( LVCGame.Vector3 v3 )
	{
		return DisplayFormatted( v3,"m/s" );
	}
	
	/**
	 * A utility function to create a string using an LVC Vector3 to display the values of the 3 axes 
	 * as metres per second squared (acceleration). This is used for logging purposes.
	 * 
	 * @param v3 the LVC Vector3 containing the acceleration values for each axis
	 * @return the formatted string
	 */
	public static string DisplayAsMs2( LVCGame.Vector3 v3 )
	{
		return DisplayFormatted( v3,"m/s\u00B2" );
	}
	
	/**
	 * A utility function to create a string using an LVC Vector3 to display the values of the 3 axes 
	 * with unit formatting. This is used for logging purposes.
	 * 
	 * @param v3 the LVC Vector3 containing the acceleration values for each axis
	 * @param the suffix to display after each value
	 * @return the formatted string
	 */
	public static string DisplayFormatted( LVCGame.Vector3 v3, string suffix )
	{
		return "("+v3.x.ToString("f5")+suffix+", "+v3.y.ToString("f5")+suffix+", "+v3.z.ToString("f5")+suffix+")";
	}
	
	/**
	 * A utility function to create a string using an LVC Vector3 to display the values of the 3 axes 
	 * as degrees. This is used for logging purposes.
	 * 
	 * @param v3 the LVC Vector3 containing the acceleration values for each axis
	 * @return the formatted string
	 */
	public static string DisplayAsDegrees( LVCGame.Vector3 v3 )
	{
		string suffix = "\u00B0";
		return "("+Radians2Degrees(v3.x).ToString("f2")+suffix+", "+Radians2Degrees(v3.y).ToString("f2")+suffix+", "+Radians2Degrees(v3.z).ToString("f2")+suffix+")";
	}
	
	/**
	 * A utility function to create a string using an LVC Vector3 to display the values of the 3 axes 
	 * as d*m's" (degrees, minutes, seconds). This is used for logging purposes.
	 * 
	 * @param v3 the LVC Vector3 containing the acceleration values for each axis
	 * @return the formatted string
	 */
	public static string DisplayAsDMS( LVCGame.Vector3 v3 )
	{
		return "("+RadiansToDMS(v3.x, 2)+", "+RadiansToDMS(v3.y, 2)+", "+RadiansToDMS(v3.z,2)+")";
	}
	
	/**
	 * A utility function to create a string using an LVC Vector3 to display the values of the 3 axes 
	 * as decimal degrees. This is used for logging purposes.
	 * 
	 * @param v3 the LVC Vector3 containing the acceleration values for each axis
	 * @return the formatted string
	 */
	public static string DisplayAsDegreesPerSecond( LVCGame.Vector3 v3 )
	{
		string suffix = "\u00B0/s";
		return "("+Radians2Degrees(v3.x).ToString("f2")+suffix+", "+Radians2Degrees(v3.y).ToString("f2")+suffix+", "+Radians2Degrees(v3.z).ToString("f2")+suffix+")";
	}
	
	/**
	 * A utility function to create a string using an LVC EntityData struct to display the salient
	 * details of an entity's state
	 * 
	 * @param ed the entity data struct to create the string representation from
	 * @return the formatted string
	 */
	public static string DisplayFormatted( LVCGame.EntityData ed )
	{
		return "("+ed.id.instance+") "+ed.id.gameType+"["+ed.id.marking+"]"+
				                       DisplayFormatted(ed.physics.position, "")+
								       DisplayAsDegrees(ed.physics.orientation)+" "+
								       DisplayAsMs(ed.physics.worldVelocity)+" "+
								       DisplayAsMs2(ed.physics.worldAcceleration)+" "+
								       DisplayAsDegreesPerSecond(ed.physics.bodyAngularVelocity)+" "+
									   ed.properties.stance.ToString();
	}	
	
	/**
	 * A utility function to create a string using an LVC FireWeaponData struct to display the salient
	 * details
	 * 
	 * @param fwd the fire weapon data struct to create the string representation from
	 * @return the formatted string
	 */
	public static string DisplayFormatted( LVCGame.FireWeaponData fwd )
	{
		return fwd.descriptor.quantity+" rounds of "+fwd.targeting.munitionType+" from position "+
			   DisplayFormatted(LVCUtils.llaToLtp(fwd.targeting.position), "")+", velocity "+
			   DisplayAsMs(fwd.targeting.velocity);
	}	
	
	/**
	 * A utility function to create a string using an LVC FireWeaponData struct to display the salient
	 * details
	 * 
	 * @param fwd the fire weapon data struct to create the string representation from
	 * @return the formatted string
	 */
	public static string DisplayFormatted( LVCGame.DetonateMunitionData dmd )
	{
		return dmd.descriptor.quantity+" rounds of "+dmd.targeting.munitionType+" at position "+
			   DisplayFormatted(LVCUtils.llaToLtp(dmd.targeting.position), "");
	}
	
	/**
	 * A utility function to create a string using an LVC EntityData struct to display the salient
	 * details of an entity's state
	 * 
	 * @param ed the entity data struct to create the string representation from
	 * @return the formatted string
	 */
	public static string DisplayFormatted( LVCGame.UTMCoordinate utm )
	{
		return ""+System.Math.Abs(lvcWorldOriginUTM.easting)+(lvcWorldOriginUTM.easting<0?"W":"E")+" "
		         +System.Math.Abs(lvcWorldOriginUTM.northing)+(lvcWorldOriginUTM.northing<0?"S":"N")+" "
		         +lvcWorldOriginUTM.altitude+"m "
		         +"Z"+lvcWorldOriginUTM.zone+" "
		         +lvcWorldOriginUTM.hemisphere;
	}
	
	/**
	 * A utility function to obtain the DIS enumeration of an entity from its EntityData
	 * 
	 * @param entityData the LVCGame.EntityData instance
	 * @return the DIS enumeration values as an integer array of 7 values
	 */
	public static int[] GetDisEnumeration( LVCGame.EntityData entityData )
	{
		int[] result = new int[7];
		string[] disEnumParts = entityData.id.lvcType.Split(' ');
		for(int i=0;i<disEnumParts.Length;i++)
		{
			result[i] = StringToInt( disEnumParts[i], 0 );
		}
		return result;
	}
	
	/**
	 * A utility function to obtain the DIS 'kind' of an entity from its EntityData
	 * 
	 * @param entityData the LVCGame.EntityData instance
	 * @return the DIS kind value
	 */
	public static int GetDisKind( LVCGame.EntityData entityData )
	{
		string[] disEnumParts = entityData.id.lvcType.Split(' ');
		
		if( disEnumParts.Length < ((int)DIS_FIELD_INDEX.KIND+1))
			return 0;
		
		return StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.KIND], ((int)DIS_KIND.OTHER) );
	}
	
	/**
	 * A utility function to obtain the DIS 'domain' of an entity from its EntityData
	 * 
	 * @param entityData the LVCGame.EntityData instance
	 * @return the DIS domain value
	 */
	public static int GetDisDomain( LVCGame.EntityData entityData )
	{
		string[] disEnumParts = entityData.id.lvcType.Split(' ');
		
		if( disEnumParts.Length < ((int)DIS_FIELD_INDEX.DOMAIN+1))
			return 0;
		
		return StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.DOMAIN], ((int)DIS_DOMAIN.OTHER) );
	}
	
	/**
	 * A utility function to obtain the DIS 'country' of an entity from its EntityData
	 * 
	 * @param entityData the LVCGame.EntityData instance
	 * @return the DIS country value
	 */
	public static int GetDisCountry( LVCGame.EntityData entityData )
	{
		string[] disEnumParts = entityData.id.lvcType.Split(' ');
		
		if( disEnumParts.Length < ((int)DIS_FIELD_INDEX.COUNTRY+1))
			return 0;
		
		return StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.COUNTRY], 0 );
	}
	
	/**
	 * A utility function to determine if an entity is a land entity from its EntityData.
	 * It will return true if, and only if the entity is...
	 *      - a lifeform or platform, *and*
	 *      - its domain is land
	 * Note that the primarty purpose of this method is to determine whether an entity is 
	 * a suitable candidate for ground clamping position processing.
	 * 
	 * @param entityData the LVCGame.EntityData instance
	 * @return true if the entity is a land entity.
	 */
	public static bool IsLandEntity( LVCGame.EntityData entityData )
	{
		string[] disEnumParts = entityData.id.lvcType.Split(' ');
		
		if( disEnumParts.Length < ((int)DIS_FIELD_INDEX.DOMAIN+1))
			return false;
		
		int kind = StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.KIND], 0 );
		if( kind == (int)DIS_KIND.LIFE_FORM || kind == (int)DIS_KIND.PLATFORM )
		{
			int domain = StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.DOMAIN], ((int)DIS_DOMAIN.OTHER) );
			return domain == (int)DIS_DOMAIN.LAND;
		}
		return false;
	}
	
	/**
	 * A utility function to determine if a munition is an air munition from its EntityData.
	 * It will return true if, and only if the entity is...
	 *      - a muniotion, *and*
	 *      - its (munition) domain is anti-air
	 * Note that the primarty purpose of this method is to determine whether the munition is 
	 * a suitable candidate for ground clamping position processing (i.e., clamp the detonation
	 * to the ground).
	 * 
	 * @param entityData the LVCGame.EntityData instance
	 * @return true if the entity is an air munition.
	 */
	public static bool IsAirMunition( LVCGame.EntityData entityData )
	{
		string[] disEnumParts = entityData.id.lvcType.Split(' ');
		
		if( disEnumParts.Length < ((int)DIS_FIELD_INDEX.DOMAIN+1))
			return false;
		
		int kind = StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.KIND], 0 );
		if( kind == (int)DIS_KIND.MUNITION )
		{
			int domain = StringToInt( disEnumParts[(int)DIS_FIELD_INDEX.DOMAIN], ((int)DIS_DOMAIN.OTHER) );
			return domain == (int)DIS_MUNITION_DOMAIN.ANTI_AIR;
		}
		return false;
	}
	
	/**
	 * Utility method to calculate the amount by which an effect has decayed based on the distance from the source.
	 * 
	 * @param effectLevel the maximum level of the effect (i.e., at the source )
	 * @param effectRadius the effect radius of the event - outside of this radius, the effect has decayed to 0.
	 * @param distance the distance to calculate the effect level for
	 * @return the level of the effect at the provided distance.
	 */
	public static float LinearDecay( float effectLevel, float effectRadius, float distance )
	{
		float decayedValue = (1.0F-(distance/effectRadius)) * effectLevel;
		return ConstrainValue( decayedValue, 0.0F, effectLevel );
	}
	
	/**
	 * Utility method to calculate the amount by which an effect has decayed based on the distance from the source.
	 * 
	 * @param effectLevel the maximum level of the effect (i.e., at the source )
	 * @param effectRadius the effect radius of the event - outside of this radius, the effect has decayed to 0.
	 * @param distance the distance to calculate the effect level for
	 * @return the level of the effect at the provided distance.
	 */
	public static float SquareDecay( float effectLevel, float effectRadius, float distance )
	{
		float decayedValue = (1.0F-((distance*distance)/(effectRadius*effectRadius))) * effectLevel;
		return ConstrainValue( decayedValue, 0.0F, effectLevel );
	}
}
