using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour {
    public float speed;
    public Text countText;
    public Text winText;
    public Text myText;
    private Rigidbody rb;
    private int count;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";
    }

    // Update is called once per frame
    void Update () {
        float xI = Input.GetAxis("Horizontal");
        float yI = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(xI, 0, yI);
        rb.AddForce(movement*speed);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            //transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
            Vector3 movement2 = new Vector3(-touchDeltaPosition.x, 0, -touchDeltaPosition.y);
            rb.AddForce(movement2);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pu"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    public Vector3 getRandomVector() {
        float x = Random.Range(-10, 10);
        float y = Random.Range(-10, 10);
        float z = Random.Range(-10, 10);
        Vector3 pos = new Vector3(x, y, z);
        return pos;
    }

    public void createMytext(string message) {
        Text atext = Instantiate(myText);
        float x = Random.Range(-10, 10);
        float y = Random.Range(-10, 10);
        float z = Random.Range(-10, 10);
        Vector3 pos = new Vector3(x, y, z);
        GameObject canvas = new GameObject("_Canvas", typeof(Canvas));
        canvas.transform.position = transform.position;
        //canvas.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
        //tempT.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>(),false);
        atext.GetComponent<Transform>().SetParent(canvas.GetComponent<Transform>(), true);
        atext.text = message;
        atext.GetComponent<RectTransform>().localPosition = transform.position;
        atext.GetComponent<RectTransform>().localScale = new Vector3(0.1F, 0.1F, 0.1F);
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winText.text = "You Win!";
        }
    }
}
