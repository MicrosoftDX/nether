using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Nether : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static IEnumerator PostScore()
    {
        string data = "{ \"value\": \"Kristofer was here!\" }";
        WWWForm form = new WWWForm();

        form.AddField("value", "Kristofer was here!");

        UnityWebRequest wr = UnityWebRequest.Post("http://localhost:63953/api/values/", form);
        yield return wr.Send();

        if (wr.isError)
        {
            Debug.Log(wr.error);
        }
        else
        {
            Debug.Log("Score Posted!");
        }
    }
}
