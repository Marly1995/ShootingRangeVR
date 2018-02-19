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
	Rigidbody leftRB;
	[SerializeField]
	Rigidbody rightRB;
    [SerializeField]
    Transform head;

    [Space]
    [SerializeField]
    clayShooter shooter;

	[SerializeField]
	string number;

	string directory = "C:\\Users\\marlon\\Documents\\GitHub\\GalleryData\\";
	string filename = "\\globalData";

	TextWriter textWriter;
	CsvWriter csvWriter;

	TransformData data;

	private void Start()
	{
		directory += number;
		Directory.CreateDirectory (directory);
		directory += filename;
		textWriter = File.CreateText(directory);
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
				rightRB.angularVelocity.x, rightRB.angularVelocity.y, rightRB.angularVelocity.z, rightRB.velocity.x, rightRB.velocity.y, rightRB.velocity.z,
				leftHand.position.x, leftHand.position.y, leftHand.position.z, leftHand.rotation.x, leftHand.rotation.y, leftHand.rotation.z, leftHand.rotation.w,
				leftRB.angularVelocity.x, leftRB.angularVelocity.y, leftRB.angularVelocity.z, leftRB.velocity.x, leftRB.velocity.y, leftRB.velocity.z,
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
	public float RightAngluarVelocityX { get; set; }
	public float RightAngluarVelocityY { get; set; }
	public float RightAngluarVelocityZ { get; set; }
	public float RightVelocityX { get; set; }
	public float RightVelocityY { get; set; }
	public float RightVelocityZ { get; set; }
    public float LeftX { get; set; }
    public float LeftY { get; set; }
    public float LeftZ { get; set; }
    public float LeftRotX { get; set; }
    public float LeftRotY { get; set; }
    public float LeftRotZ { get; set; }
    public float LeftRotW { get; set; }
	public float LeftAngluarVelocityX { get; set; }
	public float LeftAngluarVelocityY { get; set; }
	public float LeftAngluarVelocityZ { get; set; }
	public float LeftVelocityX { get; set; }
	public float LeftVelocityY { get; set; }
	public float LeftVelocityZ { get; set; }
    public bool Gesturing { get; set; }
    public bool GestureTime { get; set; }
    public int GestureNumber { get; set; }

    public TransformData(float hx, float hy, float hz, float hrx, float hry, float hrz, float hrw, 
		float rx, float ry, float rz, float rrx, float rry, float rrz, float rrw, float ravx, float ravy, float ravz, float rvx, float rvy, float rvz,
		float lx, float ly, float lz, float llx, float lly, float llz, float llw, float lavx, float lavy, float lavz, float lvx, float lvy, float lvz,
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
		RightAngluarVelocityX = ravx;
		RightAngluarVelocityY = ravy;
		RightAngluarVelocityZ = ravz;
		RightVelocityX = rvx;
		RightVelocityY = rvy;
		RightVelocityZ = rvz;
		LeftX = lx;
        LeftY = ly;
        LeftZ = lz;
        LeftRotX = llx;
        LeftRotY = lly;
        LeftRotZ = llz;
        LeftRotW = llw;
		LeftAngluarVelocityX = lavx;
		LeftAngluarVelocityY = lavy;
		LeftAngluarVelocityZ = lavz;
		LeftVelocityX = lvx;
		LeftVelocityY = lvy;
		LeftVelocityZ = lvz;
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
		Map (m => m.RightAngluarVelocityX).Index (14);
		Map (m => m.RightAngluarVelocityY).Index (15);
		Map (m => m.RightAngluarVelocityZ).Index (16);
		Map (m => m.RightVelocityX).Index (17);
		Map (m => m.RightVelocityY).Index (18);
		Map (m => m.RightVelocityZ).Index (19);
        Map(m => m.LeftX).Index(20);
        Map(m => m.LeftY).Index(21);
        Map(m => m.LeftZ).Index(22);
        Map(m => m.LeftRotX).Index(23);
        Map(m => m.LeftRotY).Index(24);
        Map(m => m.LeftRotZ).Index(25);
        Map(m => m.LeftRotW).Index(26);
		Map(m => m.LeftAngluarVelocityX).Index(27);
		Map(m => m.LeftAngluarVelocityY).Index(28);
		Map(m => m.LeftAngluarVelocityZ).Index(29);
		Map(m => m.LeftVelocityX).Index(30);
		Map(m => m.LeftVelocityY).Index(31);
		Map(m => m.LeftVelocityZ).Index(32);
		Map(m => m.Gesturing).Index(33);
        Map(m => m.GestureTime).Index(34);
        Map(m => m.GestureNumber).Index(35);
    }
}