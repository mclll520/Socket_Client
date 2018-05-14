/*              #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###              
         ##       ###        ###               
__________#_______####_______####______________
    身是菩提树，心如明镜台，时时勤拂拭，勿使惹尘埃。
                我们的未来没有BUG              
* ==============================================================================
* Filename: GlobalConfig
* Created:  $time$
* Author:   WYC
* Purpose:  全局配置
* ==============================================================================
*/
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class GlobalConfig : MonoBehaviour
{
    [Header("服务器IP地址")]
    public string IP = "192.168.2.224";
    private Socket socketclient = null;
    private byte[] _bytes;

    [Header("声音片段")]
    public AudioClip[] MuicsClip;
    private AudioSource ClipMusic;

    [Header("开始按钮")]
    public GameObject StartButton;
    [Header("开始提示")]
    public GameObject PromptImage;
    [Header("拍照按钮")]
    public  GameObject TakingPicturesButton;
    [Header("确认按钮")]
    public  GameObject ConfirmButton;
    [Header("重拍按钮")]
    public  GameObject ToShootButton;
    [Header("合成中")]
    public GameObject InTheSynthesisOf;
    [Header("背景底图")]
    public GameObject Background;


    [Header("背景动画")]
    public GameObject BGAnimation;
    [Header("人脸动画")]
    public GameObject FaceAnimation;
    [Header("倒计时动画")]
    public GameObject CountdownAnimation;
    [Header("蛋动画")]
    public  GameObject EggAnimation;

   
    [Header("拍照像素设置")]
    public Camera MainCamera;
    private Rect rect = new Rect(0, 0, 1536, 2048);

    [Header("调用Ipad摄像头")]
    public RawImage IpadCameraTexture;
    private float aspect = 9f / 16f;
    private string m_deviceName;
    private WebCamTexture webcamTexFront;
    public WebCamTexture WebcamTexFront
    {
        get
        {
            if (webcamTexFront == null)
            {
                // 检查设备上有多少个摄像头 
                foreach (WebCamDevice device in WebCamTexture.devices)
                {
                    if (device.isFrontFacing)
                    {
                        m_deviceName = device.name;
                        webcamTexFront = new WebCamTexture(m_deviceName, Screen.width, (int)(Screen.width * aspect));
                    }
                }
            }
            IpadCameraTexture.texture = webcamTexFront;
            return webcamTexFront;
        }
    }

    void Start () {
        ClipMusic = this.GetComponent<AudioSource>();
        StartButton.GetComponent<Button>().onClick.AddListener(StartButtons);
        TakingPicturesButton.GetComponent<Button>().onClick.AddListener(TakingPicturesButtons);
        ConfirmButton.GetComponent<Button>().onClick.AddListener(ConfirmButtons);
        ToShootButton.GetComponent<Button>().onClick.AddListener(ToShootButtons);

       // btnConnection_Click();
    }

    private void PlayAudioClip(int i,bool IsLoop = false) {
        ClipMusic.clip = MuicsClip[i];
        ClipMusic.Play();
        if (IsLoop)
        {
            ClipMusic.loop = true;
        }
        else
        {
            ClipMusic.loop = false;
        }
    }

    /// <summary>
    /// 待机开始按钮
    /// </summary>
    private void StartButtons()
    {
        StartCoroutine(CloseBackground());
    }
    private IEnumerator CloseBackground()
    {
        PlayAudioClip(2);
        PromptImage.SetActive(false);
        StartButton.SetActive(false);
        BGAnimation.SetActive(false);

        IpadCameraTexture.gameObject.SetActive(true);
        FaceAnimation.SetActive(true);
        WebcamTexFront.Play();
        yield return new WaitForSeconds(2f);
        TakingPicturesButton.SetActive(true);
    }

    /// <summary>
    /// 开始拍照
    /// </summary>
    public void TakingPicturesButtons() {
        PlayAudioClip(0);

        TakingPicturesButton.SetActive(false);

        CountdownAnimation.SetActive(true);

        StartCoroutine(TakingPicturesButtonsTime());
    }
    IEnumerator TakingPicturesButtonsTime()
    {
        yield return new WaitForSeconds(4f);
        PlayAudioClip(3);
        ConfirmButton.SetActive(true);
        ToShootButton.SetActive(true);
        WebcamTexFront.Pause();
        OnClickTakePhone();

        CountdownAnimation.SetActive(false);
    }

    /// <summary>
    /// 重拍按钮
    /// </summary>
    public void ToShootButtons()
    {
        PlayAudioClip(4);
        ConfirmButton.SetActive(false);
        ToShootButton.SetActive(false);
        WebcamTexFront.Play();

        TakingPicturesButton.SetActive(true);
    }

    /// <summary>
    /// 确认上传
    /// </summary>
    private void ConfirmButtons()
    {
        PlayAudioClip(1,true);

        FaceAnimation.SetActive(false);
        ConfirmButton.SetActive(false);
        ToShootButton.SetActive(false);
        IpadCameraTexture.gameObject.SetActive(false);

        InTheSynthesisOf.SetActive(true);
        EggAnimation.SetActive(true);

        StartCoroutine(EggButtonsTime());
    }
  
    IEnumerator EggButtonsTime()
    {
        yield return new WaitForSeconds(12f);
        //SendByte();
        GlobalConfigInit();
    }

    public void GlobalConfigInit()
    {
        ClipMusic.clip = null;

        EggAnimation.SetActive(false);
        InTheSynthesisOf.SetActive(false);

        PromptImage.SetActive(true);
        StartButton.SetActive(true);
        BGAnimation.SetActive(true);
    }

    /// <summary>
    /// 抓拍像素数据
    /// </summary>
    public void OnClickTakePhone()
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        MainCamera.targetTexture = rt;
        MainCamera.Render();
        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();
        // 重置相关参数，以使用camera继续在屏幕上显示  
        MainCamera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.Destroy(rt);
        _bytes = screenShot.EncodeToJPG();
    }

    /// <summary>
    /// TCP_客户端
    /// </summary>
    public void btnConnection_Click()
    {
        //定义一个套接字监听  
        socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //获取文本框中的IP地址  
        IPAddress address = IPAddress.Parse(IP);

        //将获取的IP地址和端口号绑定在网络节点上  
        IPEndPoint point = new IPEndPoint(address, 8080);

        try
        {
            //客户端套接字连接到网络节点上，用的是Connect  
            socketclient.Connect(point);
            Debug.Log("连接成功");
        }
        catch (Exception)
        {
            Debug.Log("连接失败");
            return;
        }
    }

    void SendByte()
    {
        while (true)
        {
            if (socketclient.Poll(10, SelectMode.SelectWrite))
            {
                Debug.Log("与服务器断开");
                break;//跳出死循环
            }
        }
        socketclient.Send(_bytes);
    }
    void OnDestroy()
    {
        socketclient.Close();
    }
}
