using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DogController : MonoBehaviour {
    // Start is called before the first frame update
    Animator dogAnimator;
    Rigidbody rb;
    Vector3 position;
    Vector3 rotation;
    public Vector3 speed = new Vector3 (0, 0, 4);
    public Vector3 horzSpeed = new Vector3 (2, 0, 0);
    public Vector3 rotaSpeed = new Vector3 (0, 30, 0);
    private bool isRunning = false, isAtk = false,
        isMoveLeft = false, isMoveRight = false,
        isTurnLeft = false, isTurnRight = false;

    void Start () {
        dogAnimator = this.GetComponent<Animator> ();
        rb = this.GetComponent<Rigidbody> ();
        position = this.transform.localPosition;
        EventCenter.Instance.AddEventListener ("keyKeep", KeyKeepDogAction);
        EventCenter.Instance.AddEventListener ("keyUp", KeyUpDogAction);
        EventCenter.Instance.AddEventListener ("keyDown", KeyDownDogAction);
        rotation = new Vector3 (0, 0, 0);
    }

    private void FixedUpdate () {
        Move ();
        if (isAtk) {
            Atk ();
            isAtk = false;
        }

    }

    private void Move () {
        if (isTurnLeft || isTurnRight || isMoveLeft || isMoveRight) {
            dogAnimator.SetBool ("isWalk", true);
        }
        else {
            dogAnimator.SetBool ("isWalk", false);
        }
        if (isRunning) {
            Run ();
        }
        if (isRunning is false) {
            StopRun ();
        }
        if (isTurnLeft) {
            TurnLeft ();
        }
        if (isTurnRight) {
            TurnRight ();
        }
        if (isMoveLeft) {
            MoveLeft ();
        }
        if (isMoveRight) {
            MoveRight ();
        }
        //dogAnimator.SetBool ("isWalk", false);
    }

    // Update is called once per frame
    public void Run () {
        if (dogAnimator.GetBool ("isRunning") is false) {
            dogAnimator.SetBool ("isRunning", true);
        }
        if (dogAnimator.GetBool ("isAtk1") is false &&
            dogAnimator.GetBool ("isAtk2") is false) {
            this.transform.Translate (speed * Time.deltaTime);
        }
    }
    public void StopRun () {
        if (dogAnimator.GetBool ("isRunning") is true) {
            dogAnimator.SetBool ("isRunning", false);
        }
    }
    public void TurnLeft () {
        this.transform.Rotate (-rotaSpeed * Time.deltaTime);
    }
    public void TurnRight () {
        this.transform.Rotate (rotaSpeed * Time.deltaTime);
    }
    public void MoveLeft () {
        if (dogAnimator.GetBool ("isAtk1") is false &&
            dogAnimator.GetBool ("isAtk2") is false) {
            this.transform.Translate (-horzSpeed * Time.deltaTime);
        }
    }
    public void MoveRight () {
        if (dogAnimator.GetBool ("isAtk1") is false &&
            dogAnimator.GetBool ("isAtk2") is false) {
            this.transform.Translate (horzSpeed * Time.deltaTime);
        }
    }

    public void Atk () {
        StopCoroutine ("GoBattleFromAtk");
        StopCoroutine ("StopAtk");
        StopCoroutine ("StopBattle");
        if (dogAnimator.GetBool ("isAtk1") is true) {
            dogAnimator.SetBool ("isAtk2", true);
        } else {
            dogAnimator.SetBool ("isAtk1", true);
        }
        StartCoroutine ("GoBattleFromAtk");
        StartCoroutine ("StopAtk");
        StartCoroutine ("StopBattle");
    }

    public void EndAtk1 () {
        dogAnimator.SetBool ("isAtk1", false);
    }

    public void EndAtk2 () {
        dogAnimator.SetBool ("isAtk2", false);
    }

    private void KeyDownDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.J) {
            isAtk = true;
        }
    }

    private void KeyUpDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.W) {
            isRunning = false;
        }
        if (key == KeyCode.A) {
            isMoveLeft = false;
        }
        if (key == KeyCode.D) {
            isMoveRight = false;
        }
        if (key == KeyCode.Q) {
            isTurnLeft = false;
        }
        if (key == KeyCode.E) {
            isTurnRight = false;
        }
        if (key == KeyCode.J) {
            isAtk = false;
        }
    }

    private void KeyKeepDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.W) {
            isRunning = true;
        }
        if (key == KeyCode.A) {
            isMoveLeft = true;
        }
        if (key == KeyCode.D) {
            isMoveRight = true;
        }
        if (key == KeyCode.Q) {
            isTurnLeft = true;
        }
        if (key == KeyCode.E) {
            isTurnRight = true;
        }
    }

    IEnumerator StopAtk () {
        yield return new WaitForSeconds (1f);
        dogAnimator.SetBool ("isAtk1", false);
        dogAnimator.SetBool ("isAtk2", false);
    }
    IEnumerator StopBattle () {
        yield return new WaitForSeconds (3f);
        dogAnimator.SetBool ("isBattle", false);
    }
    IEnumerator GoBattleFromAtk () // 从第一段攻击回到备战状态
    {
        yield return new WaitForSeconds (0.35f);
        dogAnimator.SetBool ("isBattle", true);
    }
}