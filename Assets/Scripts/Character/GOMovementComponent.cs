using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Characters
{

public class GOMovementComponent : MovementComponent
{
    public float UpDuration;
    public AnimationCurve UpCurve;
    public float MoveDuration;
    public AnimationCurve MoveCurve;

    public float RotationDuration;
    public float RotationCurve;
    public float RespiteDuration;
    public Vector3 StartOffset;


    [HideInInspector]
    public Vector3 StartPosition;
    [HideInInspector]
    public Quaternion StartRotation;
    [HideInInspector]
    private Vector3 StartUpPosition;
    [HideInInspector]
    private Vector3 TargetUpPosition;
    [HideInInspector]
    public Vector3 TargetPosition;

    [HideInInspector]
    public bool InTransit = false;

    private GameObject Model;
    float time;

    

    async Task MoveUp()
    {
        float mytime = 0;
        while (mytime <= UpDuration)
        {
            float value = UpCurve.Evaluate(mytime/UpDuration);
            // Debug.Log("Value is "+ value);
            // float value = time / Duration;
            mytime += Time.deltaTime;
            transform.position = Vector3.Lerp(StartPosition, StartUpPosition, value);
            await Task.Yield();
        }

        transform.position = StartUpPosition;
    }

    public float RotationSpeed;

    async Task RotateToTarget()
    {
        Quaternion TargetRotation = Quaternion.LookRotation(TargetPosition - StartPosition, Vector3.up);
        TargetRotation = Quaternion.Euler(0, TargetRotation.eulerAngles.y, 0);
        
        float factor = 0;
        //Rotate towards target at a certain speed
        Vector3 tF = this.transform.up;
        Vector3 distance = TargetPosition - StartPosition;
        float dot = Vector3.Dot(tF, distance);
        float angle = Mathf.Acos(dot / (tF.magnitude * distance.magnitude)) *Mathf.Rad2Deg -90;
        Vector3 Cross = Vector3.Cross(StartPosition, TargetPosition);
        
        float difference = Quaternion.Angle(StartRotation, TargetRotation);
       

        if (angle <= AngleThreshold)
        {
            // Debug.Log("Angle too small");
            transform.rotation = TargetRotation;
            return;
        }

        float step = 0;
        float FullSpeed = difference*RotationSpeed;
        while (factor<=1)
        {
            factor += FullSpeed*Time.deltaTime;
            step += RotationSpeed*Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(StartRotation, TargetRotation, factor);
            await Task.Yield();
        }

        transform.LookAt(TargetPosition);

    }
    async Task MoveTargetUp()
    {
        float mytime = 0;
        while (mytime <= MoveDuration)
        {
            float value = MoveCurve.Evaluate(mytime/MoveDuration);
            // Debug.Log("Value is "+ value);
            // float value = time / Duration;
            mytime += Time.deltaTime;
            transform.position = Vector3.Lerp(StartUpPosition, TargetUpPosition, value);
            await Task.Yield();
        }

        transform.position = TargetUpPosition;
    }
    async Task MoveTarget()
    {
        float mytime = 0;
        while (mytime <= UpDuration)
        {
            float value = UpCurve.Evaluate(mytime/UpDuration);
            // Debug.Log("Value is "+ value);
            // float value = time / Duration;
            mytime += Time.deltaTime;
            transform.position = Vector3.Lerp(TargetUpPosition, TargetPosition, value);
            await Task.Yield();
        }

        transform.position = TargetPosition;
    }

    public float AngleThreshold;
    private async Task AsyncMove()
    {
        
        await RotateToTarget();
        // Debug.Log("Finished orienting to movement");
        await MoveUp();
        // Debug.Log("Finished Moving up");
        await MoveTargetUp();
        // Debug.Log("Finished Moving target up");

        await MoveTarget();
        // Debug.Log("Finished Moving target");
        // work();
        MovementFinished.Invoke();
        
        await Task.Delay((int) (RespiteDuration * 1000));
        // await Task.Yield();
    }
    
    public override async Task StartMoving(Vector3 Start, Quaternion StartRotation, Vector3 Target)
    {
        StartPosition = Start;
        StartUpPosition = StartPosition + StartOffset;
        TargetPosition = Target;
        TargetUpPosition = TargetPosition + StartOffset;
        this.StartRotation = StartRotation;
        await AsyncMove();
        // await Task.Yield();
    }

    
    void Start()
    {
        this.Model = transform.Find("Model").gameObject;
    }

    
    
}

}
