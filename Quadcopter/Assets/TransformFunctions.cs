using UnityEngine;
using System.Collections;

public class TransformFunctions : MonoBehaviour
{
	public Texture fillTexture;
	public Texture emptyTexture;

	public Texture altiBackground;
	public Texture altiDial;

	public float moveSpeed = 10f;
	public float turnSpeed = 50f;

	public GUIText tempratureText;
	public GUIText humidityText;
	public GUIText altitudeText;
	public GUIText statusText;

	public GameObject needle;

	private const string placeHolder = "StandBy...";
	private AndroidJavaClass jc;

	void Start() {
		jc = new AndroidJavaClass("com.wolkabout.hexiwear.UnityPlayerActivity");
	}

	void Update ()
	{
		/*
		if (Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit(); 
	    }
       */
		if(Input.GetKey(KeyCode.UpArrow))
			transform.eulerAngles = new Vector3(90, 0, 0);

		if(Input.GetKey(KeyCode.DownArrow))
			transform.eulerAngles = new Vector3(-45, 0, 0);

		if(Input.GetKey(KeyCode.LeftArrow))
			transform.eulerAngles = new Vector3(0, 0, 45);

		if(Input.GetKey(KeyCode.RightArrow))
			transform.eulerAngles = new Vector3(0, 0, -45);

		if (jc != null) {
			string temprature = jc.GetStatic<string> ("TemperatureReading");
			if (temprature == null) {
				tempratureText.text = placeHolder;
			} else {
				tempratureText.text = temprature;
			}

			string humidity = jc.GetStatic<string> ("HumidityReading");
			if (humidity == null) {
				humidityText.text = placeHolder;
			} else {
				humidityText.text = humidity;
			}



			string acceleration = jc.GetStatic<string> ("AccelerationReadings");
			if (acceleration == null) {
				acceleration = placeHolder;
			} else {
				string[] tokens = acceleration.Split(';');
				float angleX = float.Parse (tokens [0]);
				float angleY = float.Parse (tokens [1]);
				float angleZ = float.Parse (tokens [2]);
				transform.eulerAngles = new Vector3(angleX, angleZ, angleY);
			} 

			string magnet = jc.GetStatic<string> ("MagnetReadings");
			if (magnet == null) {
				magnet = placeHolder;
			}else {
				float angle = float.Parse (magnet);
				needle.transform.eulerAngles = new Vector3(0, 0, angle);
			}  

			string gyroscope = jc.GetStatic<string> ("GyroscopeReadings");
			if (gyroscope == null) {
				gyroscope = placeHolder;
			}

			//statusText.text = "";//acceleration + " " + magnet + " " + gyroscope;
		}
	}

	void OnGUI() {
		int currentPower = 0;

		if (jc != null) {
			string battery = jc.GetStatic<string> ("ReadingBattery");

			if (battery == null) {
				//do nothing
				//statusText.text = "null";
			} else {
				currentPower = int.Parse(battery);
			}
		}
			
		if (!fillTexture || !emptyTexture) {
			Debug.LogError("Assign a Texture in the inspector.");
			return;
		}
		int imageWidth = fillTexture.width/2;
		int imageHeight = fillTexture.height/2;
		int x = Screen.width - imageWidth -50;
		int y = Screen.height - imageHeight -50;

		Rect imageRect = new Rect(x, y,imageWidth ,imageHeight );
		GUI.DrawTexture(imageRect, emptyTexture);

		//Rect rectMask = new Rect(x,y,imageWidth ,imageHeight  );

		imageRect.width *= currentPower/100.0f; // you can divide here by 100 if you prefer to use 0 - 100%

		GUI.BeginGroup( imageRect );
		Rect imageRect2 = new Rect(0, 0,imageWidth ,imageHeight );
		GUI.DrawTexture( imageRect2, fillTexture );

		GUI.EndGroup();

		// Altitude

		float currentAlti = 0;

		if (jc != null) {
			string altistr = jc.GetStatic<string> ("PressureReading");
			if (altistr == null) {
				//do nothing
			} else {
				currentAlti = float.Parse(altistr);
			}
		}

		if (!altiBackground || !altiDial) {
			Debug.LogError("Assign a Texture in the inspector.");
			return;
		}
		float dialWidth = (int)(altiDial.width/2.5f);
		float dialHeight = (int)(altiDial.height/2.5f);
		x = 50;
		y = 220;



		float backgroundHeight = (dialHeight * currentAlti / 1000.0f);

		Rect backgroudRect = new Rect(x+15, dialHeight+y-backgroundHeight-20,dialWidth-30, backgroundHeight );

		//backgroudRect.height *= currentAlti/1000.0f; // you can divide here by 100 if you prefer to use 0 - 100%

		GUI.BeginGroup( backgroudRect );
		imageRect2 = new Rect(0, 0,dialWidth ,dialHeight );
		GUI.DrawTexture( imageRect2, altiBackground );

		GUI.EndGroup();

		imageRect = new Rect(x, y,dialWidth ,dialHeight );
		GUI.DrawTexture(imageRect, altiDial);
	}
}
