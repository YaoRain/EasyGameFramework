using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using System.Text;
using System;
public class PendulumDynamic : MonoBehaviour
{
    // parameters of ths simulation (are specified through the Unity's interface)
    public float gravity_acceleration = 9.8f;
    public float mass = 1.0f;
    public float friction_coeficient = 0.0f;
    public float initial_angular_velocity = 0.0f;
    public float time_step_h = 0.01f;
    public enum ODE_method
    {
        euler = 0,
        improved_euler = 1,
        ssprk3 = 2,
        semi_implicit = 3,
        rk4 = 4
    };
    public ODE_method ode_method;
    int i_ode_method;
    public float initial_angle = 80;
    public float rod_length = 9.8f;
    private float c = 0.0f;
    private float omega = 0.0f;
    private GameObject pendulum = null;

    // the state vector stores two entries:
    // state_vector[0] angle of pendulum (\theta) in radians
    // state_vector[1] angular velocity of pendulum
    public Vector2 state_vector;

    private void Awake() {
        EventCenter.Instance.AddEventListener ("exitPendulum",ExitPendulum);
    }

    void Start()
    {
        i_ode_method = (int)ode_method;
        time_step_h = Time.fixedDeltaTime;
        pendulum = GameObject.Find("Pendulum");
        if (pendulum == null)
        {
            Debug.LogError("Sphere not found! Did you delete it from the starter scene?");
        }

        state_vector[0] = initial_angle * Mathf.PI / 180;
        state_vector[1] = initial_angular_velocity;

        c = friction_coeficient / mass; // following the ODE specification
        omega = gravity_acceleration / rod_length;
    }

    void FixedUpdate()
    {
        float kinetic_energy = mass * (state_vector.y * rod_length) * (state_vector.y * rod_length) / 2;
        float potential_energy = mass * gravity_acceleration * rod_length * (1 - Mathf.Cos(state_vector.x));
        float energy = kinetic_energy + potential_energy;
        state_vector = OdeStep(state_vector, time_step_h, PendulumDynamics);
        pendulum.transform.eulerAngles = new Vector3(state_vector.x * 180 / Mathf.PI, 0.0f, 0f); // 弧度需转化成角度
    }

    private void OnTriggerEnter(Collider player) {
        if(player.gameObject.layer == LayerMask.NameToLayer("InputHandle")){
            Debug.Log("touchPendulum");
            state_vector.y = -player.GetComponent<Rigidbody>().velocity.magnitude/rod_length; // 根据角色的速度得到初速度
            EventCenter.Instance.TiggerEvent("onPendulum",this.gameObject);
        }else{
            Debug.Log(player.gameObject.layer);
        }
        
    }

    private void ExitPendulum(object obj){
        this.GetComponent<Collider>().enabled = false;
        StartCoroutine(EnableCollider());
    }

    IEnumerator EnableCollider(){
        yield return new WaitForSeconds(3f);
        this.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// 二阶常微分方程
    /// 二阶导数与一阶导数和原函数的函数关系，摆锤的运动学公式
    /// </summary>
    /// <param name="y_present">角位移</param>
    /// <param name="z_present">角位移的一阶导数</param>
    /// <returns>角位移的二阶导数</returns>
    float PendulumDynamics(float y_present, float z_present)
    {
        return -(omega * Mathf.Sin(y_present) + c * z_present);
    }

    /// <summary>
    /// 欧拉方法，用步进的方式求一阶常微分方程
    /// </summary>
    /// <param name="y_present">时间片内函数的初始值</param>
    /// <param name="z_present">函数的一阶导数</param>
    /// <param name="dt">时间片的长度</param>
    /// <returns>下一个时间片函数的初始值</returns>
    float Y_NEXT(float y_present, float z_present, float dt)
    {
        return y_present + dt * z_present;
    }

    /// <summary>
    /// 使用欧拉方法求平面摆运动
    /// </summary>
    /// <param name="present">平面摆在时间片内的初始状态</param>
    /// <param name="dt">时间片长度</param>
    /// <param name="z_d">二阶常微分方程</param>
    /// <returns>下一个时间片平面摆的初始状态</returns>
    Vector2 Euler(Vector2 present, float dt, Func<float, float, float> z_d)
    {
        float y_present = present.x, z_present = present.y;
        float y_next = Y_NEXT(present.x, present.y, dt);
        float z_next = Y_NEXT(z_present, z_d(y_present, z_present), dt);
        return new Vector2(y_next, z_next);
    }

    /// <summary>
    /// 改进的欧拉算法
    /// </summary>
    /// <param name="present">平面摆在时间片内的初始状态</param>
    /// <param name="dt">时间片长度</param>
    /// <returns>下一个时间片平面摆的初始状态</returns>
    Vector2 ImprovedPendulumDynamics(Vector2 present, float dt, Func<float, float, float> z_d)
    {
        /*
        实现公式
        y[k+1] = y[k] + 1/2 * (k1+k2)
        z[k+1] = z[k] + 1/2 * (l1+l2)
        k1 = z[k], l1 = f(x[k],y[k],z[k])
        k2 = z[k]+h*l1, l2 = f(x[k]+h, y[k]+h*k1, z[k]+h*l1)
        也写成 l2 = f(x[k]+h, y[k]+h*k1, k2)
        k表示一阶导数，l表示二阶导数，h表示时间片长度
        k1、k2分别表示两个不同的状态处的一阶导数
        函数f(x,y,z) 即为二阶常微分方程，既函数Z_D(y[k],z[k])
        运算y[k] + h*k1,即为欧拉方法求一阶常微分方程的过程Y_NEXT(y[k],k1,dt)
        */
        float y_present = present.x, z_present = present.y;

        float k1 = z_present;
        float l1 = z_d(y_present, z_present);

        float k2 = Y_NEXT(z_present, l1, dt);
        float y_l2 = Y_NEXT(y_present, k1, dt);
        float l2 = z_d(y_l2, k2);

        float z_fixed = (k1 + k2) / 2;
        float zd_fixed = (l1 + l2) / 2;
        float y_next = Y_NEXT(y_present, z_fixed, dt);
        float z_next = Y_NEXT(z_present, zd_fixed, dt);

        return new Vector2(y_next, z_next);
    }

    /// <summary>
    /// Strong Stability Preserving Runge-Kutta
    /// </summary>
    /// <param name="present">平面摆在时间片内的初始状态</param>
    /// <param name="dt">时间片长度</param>
    /// <returns>下一个时间片平面摆的初始状态</returns>
    Vector2 SSPRK3(Vector2 present, float dt, Func<float, float, float> z_d)
    {
        float y_present = present.x, z_present = present.y;

        float k1 = z_present;
        float l1 = z_d(y_present, z_present);

        float k2 = Y_NEXT(z_present, l1, dt);
        float y_l2 = Y_NEXT(y_present, k1, dt);
        float l2 = z_d(y_l2, k2);

        float k3 = Y_NEXT(z_present, l1 + l2, dt / 4);
        float y_l3 = Y_NEXT(y_present, k1 + k2, dt / 4);
        float l3 = z_d(y_l3, k3);

        float z_fixed = (k1 + k2 + 4 * k3) / 6;
        float zd_fixed = (l1 + l2 + 4 * l3) / 6;
        float y_next = Y_NEXT(y_present, z_fixed, dt);
        float z_next = Y_NEXT(z_present, zd_fixed, dt);

        return new Vector2(y_next, z_next);
    }

    /// <summary>
    /// Fourth-Order Runge-Kutta method
    /// </summary>
    /// <param name="present">平面摆在时间片内的初始状态</param>
    /// <param name="dt">时间片长度</param>
    /// <returns>下一个时间片平面摆的初始状态</returns>
    Vector2 RK4(Vector2 present, float dt, Func<float, float, float> z_d)
    {
        float y_present = present.x, z_present = present.y;

        float k1 = z_present;
        float l1 = z_d(y_present, z_present);

        float k2 = Y_NEXT(z_present, l1, dt / 2);
        float y_l2 = Y_NEXT(y_present, k1, dt / 2);
        float l2 = z_d(y_l2, k2);

        float k3 = Y_NEXT(z_present, l2, dt / 2);
        float y_l3 = Y_NEXT(y_present, k2, dt / 2);
        float l3 = z_d(y_l3, k3);

        float k4 = Y_NEXT(z_present, l3, dt);
        float y_l4 = Y_NEXT(y_present, k3, dt);
        float l4 = z_d(y_l4, k4);

        float z_fixed = (k1 + 2 * k2 + 2 * k3 + k4) / 6;
        float zd_fixed = (l1 + 2 * l2 + 2 * l3 + l4) / 6;
        float y_next = Y_NEXT(y_present, z_fixed, dt);
        float z_next = Y_NEXT(z_present, zd_fixed, dt);

        return new Vector2(y_next, z_next);
    }

    /// <summary>
    /// Semi-implicit Euler method 
    /// </summary>
    /// <param name="present">平面摆在时间片内的初始状态</param>
    /// <param name="dt">时间片长度</param>
    /// <returns>下一个时间片平面摆的初始状态</returns>
    Vector2 Semi_implicit_Euler(Vector2 present, float dt, Func<float, float, float> z_d)
    {
        float z_next = Y_NEXT(present.y, z_d(present.x, present.y), dt);
        float y_next = Y_NEXT(present.x, z_next, dt);
        return new Vector2(y_next, z_next);
    }

    Vector2 OdeStep(Vector2 present, float dt, Func<float, float, float> z_d)
    {
        // delete the next line, and complete this z_d 
        state_vector[0] += 0.1f; // update the state_vector (both entries) properly depending on the specified ode_method 

        if (i_ode_method == 0)
        {
            return Euler(present, dt, z_d);
        }
        else if (i_ode_method == 1)
        {
            return ImprovedPendulumDynamics(present, dt, z_d);
        }
        else if (i_ode_method == 2)
        {
            return SSPRK3(present, dt, z_d);
        }
        else if (i_ode_method == 3)
        {
            return Semi_implicit_Euler(present, dt, z_d);
        }
        else if (i_ode_method == 4)
        {
            return RK4(present, dt, z_d);
        }
        else
        {
            Debug.LogError("ODE method should be one of the: euler, improved-euler, rk, semi-implicit");
            return new Vector2(80, 0);
        }
    }
}
