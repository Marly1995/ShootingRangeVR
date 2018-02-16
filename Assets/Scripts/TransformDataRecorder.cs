using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;
using System.IO;

public class TransformDataRecorder : MonoBehaviour
{
    [SerializeField]
    Transform rightHand;
    [SerializeField]
    Transform leftHand;
    [SerializeField]
    Transform head;

    [Space]
    [SerializeField]
    clayShooter shooter;

    [SerializeField]
    string file;

    TextWriter textWriter;
    CsvWriter csvWriter;

    TransformData data;

    private void Start()
    {
        textWriter = File.CreateText(file);
        csvWriter = new CsvWriter(textWriter);
        csvWriter.Configuration.RegisterClassMap<TransformDataMap>();
        csvWriter.WriteHeader<TransformData>();
        csvWriter.NextRecord();
    }

    private void FixedUpdate()
    {
        if (shooter.started)
        {
            data = new TransformData(head.position.x, head.position.y, head.position.z, head.rotation.x, head.rotation.y, head.rotation.z, head.rotation.w,
                                    rightHand.position.x, rightHand.position.y, rightHand.position.z, rightHand.rotation.x, rightHand.rotation.y, rightHand.rotation.z, rightHand.rotation.w,
                                    leftHand.position.x, leftHand.position.y, leftHand.position.z, leftHand.rotation.x, leftHand.rotation.y, leftHand.rotation.z, leftHand.rotation.w,
                                    shooter.Gesturing, shooter.GestureTime, shooter.gestureNumber);
            csvWriter.WriteRecord(data);
            csvWriter.NextRecord();
        }
    }
}

public class TransformData
{
    public float HeadX { get; set; }
    public float HeadY { get; set; }
    public float HeadZ { get; set; }
    public float HeadRotX { get; set; }
    public float HeadRotY { get; set; }
    public float HeadRotZ { get; set; }
    public float HeadRotW { get; set; }
    public float RightX { get; set; }
    public float RightY { get; set; }
    public float RightZ { get; set; }
    public float RightRotX { get; set; }
    public float RightRotY { get; set; }
    public float RightRotZ { get; set; }
    public float RightRotW { get; set; }
    public float LeftX { get; set; }
    public float LeftY { get; set; }
    public float LeftZ { get; set; }
    public float LeftRotX { get; set; }
    public float LeftRotY { get; set; }
    public float LeftRotZ { get; set; }
    public float LeftRotW { get; set; }
    public bool Gesturing { get; set; }
    public bool GestureTime { get; set; }
    public int GestureNumber { get; set; }

    public TransformData(float hx, float hy, float hz, float hrx, float hry, float hrz, float hrw, 
                        float rx, float ry, float rz, float rrx, float rry, float rrz, float rrw, 
                        float lx, float ly, float lz, float llx, float lly, float llz, float llw, 
                        bool gesture, bool gestureTime, int gestureNumber)
    {
        HeadX = hx;
        HeadY = hy;
        HeadZ = hz;
        HeadRotX = hrx;
        HeadRotY = hry;
        HeadRotZ = hrz;
        HeadRotW = hrw;
        RightX = rx;
        RightY = ry;
        RightZ = rz;
        RightRotX = rrx;
        RightRotY = rry;
        RightRotZ = rrz;
        RightRotW = rrw;
        LeftX = lx;
        LeftY = ly;
        LeftZ = lz;
        LeftRotX = llx;
        LeftRotY = lly;
        LeftRotZ = llz;
        LeftRotW = llw;
        Gesturing = gesture;
        GestureTime = gestureTime;
        GestureNumber = gestureNumber;
    }
}

public sealed class TransformDataMap : ClassMap<TransformData>
{
    public TransformDataMap()
    {
        Map(m => m.HeadX).Index(0);
        Map(m => m.HeadY).Index(1);
        Map(m => m.HeadZ).Index(2);
        Map(m => m.HeadRotX).Index(3);
        Map(m => m.HeadRotY).Index(4);
        Map(m => m.HeadRotZ).Index(5);
        Map(m => m.HeadRotW).Index(6);
        Map(m => m.RightX).Index(7);
        Map(m => m.RightY).Index(8);
        Map(m => m.RightZ).Index(9);
        Map(m => m.RightRotX).Index(10);
        Map(m => m.RightRotY).Index(11);
        Map(m => m.RightRotZ).Index(12);
        Map(m => m.RightRotW).Index(13);
        Map(m => m.LeftX).Index(14);
        Map(m => m.LeftY).Index(15);
        Map(m => m.LeftZ).Index(16);
        Map(m => m.LeftRotX).Index(17);
        Map(m => m.LeftRotY).Index(18);
        Map(m => m.LeftRotZ).Index(19);
        Map(m => m.LeftRotW).Index(20);
        Map(m => m.Gesturing).Index(21);
        Map(m => m.GestureTime).Index(22);
        Map(m => m.GestureNumber).Index(23);
    }
}