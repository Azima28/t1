using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float length, startpos;
    public Transform cam;
    
    [Tooltip("0 = Bergerak sama dengan kamera (Background jauh). 1 = Bergerak seolah-olah diam di tempat (Foreground dekat).")]
    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        
        if (cam == null)
        {
            cam = Camera.main.transform;
        }
    }

    void Update()
    {
        // Hitung pergeseran relatif terhadap kamera
        float temp = (cam.position.x * (1 - parallaxEffect));
        float dist = (cam.position.x * parallaxEffect);

        // Pindahkan posisi sprite
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // Logika Looping (Infinite Scrolling)
        if (temp > startpos + length)
        {
            startpos += length;
        }
        else if (temp < startpos - length)
        {
            startpos -= length;
        }
    }
}
