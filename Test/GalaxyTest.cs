using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Game.Galaxy;
using UnityEngine;

public class GalaxyTest : MonoBehaviour
{
    [SerializeField]
    Transform m_Star = null;
    float m_Speed = 30;

    TezGalaxySimulator m_Simulator = new TezGalaxySimulator();

    #region 相机控制
    Vector3 m_CameraAngle;
    float m_CameraHeight = 2.5f;
    #endregion

    void Start()
    {
        m_CameraAngle = this.transform.eulerAngles;

        m_Simulator.generate(600, 40, 0.0004f, 0.85f, 0.95f, 2000);
        m_Simulator.foreachStar((TezStar star) =>
        {
            var star_renderer = Instantiate(m_Star);
            star_renderer.gameObject.SetActive(true);
            star_renderer.position = star.calculateOrbit();
            var rate = star.temperature / 6000f;
            star_renderer.GetComponent<MeshRenderer>().material.color = Color.Lerp(
                Color.Lerp(Color.blue, Color.red, rate),
                Color.white,
                rate);
            star.usrdata = star_renderer;
        });
    }

    private void move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.localPosition += transform.forward * m_Speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.localPosition -= transform.forward * m_Speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.localPosition -= transform.right * m_Speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.localPosition += transform.right * m_Speed * Time.deltaTime;
        }

        float y = Input.GetAxis("Mouse X");
        float x = Input.GetAxis("Mouse Y");

        m_CameraAngle.x -= x;
        m_CameraAngle.y += y;

        this.transform.eulerAngles = m_CameraAngle;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);

//         float camy = m_CameraAngle.y;
//         this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, camy, this.transform.eulerAngles.z);
    }

    void Update()
    {
        this.move();


        m_Simulator.foreachStar((TezStar star) =>
        {
            star.theta += star.velocityTheta * Time.deltaTime * 50000;
            var star_renderer = (Transform)star.usrdata;
            star_renderer.position = star.calculateOrbit();
        });
    }
}
