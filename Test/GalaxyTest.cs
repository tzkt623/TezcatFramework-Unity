using tezcat.Framework.Game.Galaxy;
using tezcat.Framework.GraphicSystem;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyTest : MonoBehaviour
{
    [SerializeField]
    Transform m_Star = null;
    [SerializeField]
    Transform m_Dust = null;

    [Header("Data")]
    [SerializeField]
    Slider m_AngleOffset = null;
    [SerializeField]
    Slider m_CoreRadius = null;
    [SerializeField]
    Slider m_GalaxyRadius = null;
    [SerializeField]
    Slider m_InnerExcentricity = null;
    [SerializeField]
    Slider m_OuterExcentricity = null;
    [SerializeField]
    Slider m_Arms = null;
    [SerializeField]
    Slider m_N = null;

    float m_Speed = 30;

    TezGalaxySimulator m_Simulator = new TezGalaxySimulator();

    TezGraphicSystem m_Graphic = null;

    #region 相机控制
    Vector3 m_CameraAngle;
    float m_CameraHeight = 2.5f;
    #endregion

    int m_ColorCount = 200;
    Color[] m_Colors = null;
    float m_T0 = 1000;
    float m_T1 = 10000;

    void Start()
    {
        m_AngleOffset.onValueChanged.AddListener(onAngleOffsetChanged);
        m_CoreRadius.onValueChanged.AddListener(onCoreRadiusChanged);
        m_GalaxyRadius.onValueChanged.AddListener(onGalaxyRadiusChanged);
        m_InnerExcentricity.onValueChanged.AddListener(onInnerExcentricityChanged);
        m_OuterExcentricity.onValueChanged.AddListener(onOuterExcentricityChanged);
        m_Arms.onValueChanged.AddListener(onArmsChanged);
        m_N.onValueChanged.AddListener(onNChanged);

        m_CameraAngle = this.transform.eulerAngles;

        var speactra = new TezGalaxySpectra();
        m_Colors = new Color[m_ColorCount];
        float dt = (m_T1 - m_T0) / m_ColorCount;
        float x, y, z;
        for (int i = 0; i < m_ColorCount; ++i)
        {
            Color color = new Color(0, 0, 0, 1);
            speactra.bbTemp = m_T0 + dt * i;
            speactra.spectrum_to_xyz(speactra.bb_spectrum, out x, out y, out z);
            speactra.xyz_to_rgb(TezGalaxySpectra.SMPTEsystem, x, y, z, out color.r, out color.g, out color.b);
            speactra.norm_rgb(ref color.r, ref color.g, ref color.b);
            m_Colors[i] = color;
        }

        m_Simulator.generate(400, 50, 0.012f, 0.75f, 0.9f, 2000);
        m_Simulator.foreachStar((TezGalaxyBody star) =>
        {
            var renderer = Instantiate(m_Star);
            renderer.name = "Star";
            renderer.gameObject.SetActive(true);
            renderer.position = star.calculateOrbit();
            star.usrdata = renderer;

            var color = new Color(0.2f, 0.2f, 0.2f, 1.0f) + this.colorFromTemperature(star.temperature);
//            var color = this.colorFromTemperature(star.temperature) * star.brigtness;
            renderer.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
            renderer.GetComponent<MeshRenderer>().material.color = color;
        });

        m_Simulator.foreachDust((TezGalaxyBody dust) =>
        {
            var renderer = Instantiate(m_Dust);
            renderer.name = "Dust";
            renderer.gameObject.SetActive(true);
            renderer.position = dust.calculateOrbit();
            dust.usrdata = renderer;

            var color = this.colorFromTemperature(dust.temperature);
            renderer.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
            renderer.GetComponent<MeshRenderer>().material.color = color;
        });

        m_Simulator.foreachH2((TezGalaxyBody h2) =>
        {
            var renderer = Instantiate(m_Star);
            renderer.name = "H2";
            renderer.gameObject.SetActive(true);
            renderer.position = h2.calculateOrbit();
            h2.usrdata = renderer;

            var color = this.colorFromTemperature(h2.temperature) * new Color(2, 0.5f, 0.5f);
            renderer.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
            renderer.GetComponent<MeshRenderer>().material.color = color;
        });

        //         m_Graphic = new TezGraphicSystem();
        //         int count = 40;
        //         for (int i = 0; i < count; i++)
        //         {
        //             m_Graphic.drawEllipse(Vector3.zero, 20 + i * 2, 10 + i * 2, 200, i / (float)count * 360.0f, Color.red, null);
        //         }
    }

    private Color colorFromTemperature(float temp)
    {
        int index = (int)((temp - m_T0) / (m_T1 - m_T0) * m_ColorCount);
        index = Mathf.Min(m_ColorCount - 1, index);
        index = Mathf.Max(0, index);
        return m_Colors[index];
    }

    private void onNChanged(float value)
    {
        m_Simulator.pertAmp = value;
    }

    private void onArmsChanged(float value)
    {
        m_Simulator.pertN = (int)value;
    }

    private void onAngleOffsetChanged(float value)
    {
        m_Simulator.angleOffset = value;
    }

    private void onCoreRadiusChanged(float value)
    {
        m_Simulator.radiusCore = value;
    }

    private void onGalaxyRadiusChanged(float value)
    {
        m_Simulator.radiusGalaxy = value;
    }

    private void onInnerExcentricityChanged(float value)
    {
        m_Simulator.innerExcentricity = value;
    }

    private void onOuterExcentricityChanged(float value)
    {
        m_Simulator.outerExcentricity = value;
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
        //        this.move();

        var speed = Time.deltaTime * 100000;

        m_Simulator.foreachStar((TezGalaxyBody star) =>
        {
            star.theta += star.velocityTheta * speed;
            var star_renderer = (Transform)star.usrdata;
            star_renderer.position = star.calculateOrbit();
        });

        m_Simulator.foreachDust((TezGalaxyBody dust) =>
        {
            dust.theta += dust.velocityTheta * speed;
            var renderer = (Transform)dust.usrdata;
            renderer.position = dust.calculateOrbit();
        });

        m_Simulator.foreachH2((TezGalaxyBody star) =>
        {
            star.theta += star.velocityTheta * speed;
            var renderer = (Transform)star.usrdata;
            renderer.position = star.calculateOrbit();
        });
    }
}
