using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;
using System.IO;

public class LocalTransformDataTracker : MonoBehaviour
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
            data = new TransformData(head.localPosition.x, head.localPosition.y, head.localPosition.z, head.localRotation.x, head.localRotation.y, head.localRotation.z, head.localRotation.w,
				rightHand.localPosition.x, rightHand.localPosition.y, rightHand.localPosition.z, rightHand.localRotation.x, rightHand.localRotation.y, rightHand.localRotation.z, rightHand.localRotation.w,
				rightRB.angularVelocity.x, rightRB.angularVelocity.y, rightRB.angularVelocity.z, rightRB.velocity.x, rightRB.velocity.y, rightRB.velocity.z,
				leftHand.localPosition.x, leftHand.localPosition.y, leftHand.localPosition.z, leftHand.localRotation.x, leftHand.localRotation.y, leftHand.localRotation.z, leftHand.localRotation.w, 
				leftRB.angularVelocity.x, leftRB.angularVelocity.y, leftRB.angularVelocity.z, leftRB.velocity.x, leftRB.velocity.y, leftRB.velocity.z,
                                    shooter.Gesturing, shooter.GestureTime, shooter.gestureNumber);
            csvWriter.WriteRecord(data);
            csvWriter.NextRecord();
        }
    }
}