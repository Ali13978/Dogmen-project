using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GogoGaga.TME
{

    public class LeantweenCustomAnimator : MonoBehaviour
    {
        [System.Serializable]
        public class UpdateEventVector : UnityEvent<Vector3> { };
        [System.Serializable]
        public class UpdateEventfloat : UnityEvent<float> { };
        public enum MOTION_TYPE { _scale, _move, _rotate, _fade, _fadeCanvasGroup, _spin };
        
        public enum TARGET_TYPE { vector3, transform }

        public enum AXIS { Forward, Backward, Up, Down, Right, Left }

        public string Name;

        [Tooltip("What type of animation you want to execute")]
        public MOTION_TYPE AnimationType;


        [Tooltip("Whether or not animation play automatically, or you can call PlayAnimation() function")]
        public bool PlayAutomatic = true;

        

        [Tooltip("Set these properties before animation starts")]
        public bool OverrideStartValues = true;

        public TARGET_TYPE StartTargetType;

        public Transform StartTargetTranform;

        public Vector3 start_vector;

        public float startValue;
        

        [Tooltip("If this is true, it will makes the current values of tranform/canvas as the end values")]
        public bool DontOverrideEndProperty;


        [Tooltip("Set these properties for animation ending")]

        public TARGET_TYPE EndTargetType;

        public Transform EndTargetTranform;

        public Vector3 end_vector;

        public float endValue;


        [Tooltip("On Which Axis should the object rotate")]
        public AXIS RotationAxis;
        public float RotationPerSec;


        public float _delay;

        public float _time;


        public LeanTweenType _easeType;



        [Tooltip("Whether or not stop any other tween on this object when playing this")]
        public bool StopOtherTweensOnThis;
        [Tooltip("Whether or not destroy gameobject after animation")]
        public bool DestroyOnCompletion = false;

        [Tooltip("Turn it of if you are pausing game using timescale = 0, this will make sure to run animation independent of timescale")]
        public bool PlayInDeltaTime = true;

        public UnityEvent OnStart;

        public UpdateEventVector OnUpdate_Vector;
        public UpdateEventfloat OnUpdate_float;


        public UnityEvent OnCompletion;

       

        CanvasGroup _canvasGroup;

        RectTransform _rect;

        private void Awake()
        {
            GetAllComponents();
        }

        void Start()
        {

        }

        void GetAllComponents()
        {
            _rect = GetComponent<RectTransform>();

            switch (AnimationType)
            {
                case MOTION_TYPE._fadeCanvasGroup:

                    if (_rect != null)
                        _canvasGroup = GetComponent<CanvasGroup>();

                    break;
            }
        }

        void OnEnable()
        {
            if (PlayAutomatic)
                PlayAnimation();
        }

        Vector3 startTarget;
        Vector3 Endtarget;

        public void PlayAnimation()
        {
            if (StopOtherTweensOnThis)
                LeanTween.cancel(gameObject);

            switch (AnimationType)
            {
                #region Scale
                case MOTION_TYPE._scale: // Scale

                    //Setting END value

                    Endtarget = transform.lossyScale;

                    if (!DontOverrideEndProperty)
                    {
                        if (EndTargetType == TARGET_TYPE.transform)
                        {
                            if (EndTargetTranform == null)
                            {
                                Debug.LogError("End target transform is empty");
                                return;
                            }
                            else
                                Endtarget = EndTargetTranform.lossyScale;
                        }
                        else if (EndTargetType == TARGET_TYPE.vector3 && !DontOverrideEndProperty)
                        {
                            Endtarget = end_vector;
                        }
                    }

                    //Setting START value

                    if (OverrideStartValues)
                    {
                        startTarget = start_vector;

                        if (StartTargetType == TARGET_TYPE.transform)
                        {
                            if (StartTargetTranform == null)
                            {
                                Debug.LogError("Start target transform is empty");
                                return;
                            }
                            else
                                startTarget = StartTargetTranform.lossyScale;
                        }

                        transform.localScale = startTarget;
                    }



                    //Animation

                    LeanTween.scale(gameObject, Endtarget, _time)
                        .setDelay(_delay)
                        .setEase(_easeType)
                        .setOnStart(() => { OnStart.Invoke(); })
                        .setOnUpdate((Vector3 val) => { OnUpdate_Vector.Invoke(val); })
                        .setOnComplete(() => { OnCompletion.Invoke(); })
                        .setIgnoreTimeScale(!PlayInDeltaTime)
                        .setLoopClamp(1)
                        .destroyOnComplete = DestroyOnCompletion;


                    break;

                #endregion


                #region Move
                case MOTION_TYPE._move: // Move

                    //Setting END value


                    Endtarget = transform.position;

                    if (!DontOverrideEndProperty)
                    {
                        if (EndTargetType == TARGET_TYPE.transform)
                        {
                            if (EndTargetTranform == null)
                            {
                                Debug.LogError("End target transform is empty");
                                return;
                            }
                            else
                            {
                                if (_rect == null)
                                    Endtarget = EndTargetTranform.position;
                                else
                                {
                                    RectTransform otherect = EndTargetTranform.GetComponent<RectTransform>();
                                    if (otherect != null)
                                        Endtarget = otherect.anchoredPosition;
                                    else
                                        Debug.LogError("End target should have a rect tranform if using this script on a UI object or canvas object");
                                }
                            }
                        }
                        else if (EndTargetType == TARGET_TYPE.vector3)
                        {
                            Endtarget = end_vector;
                        }
                    }


                    //Setting START value

                    if (OverrideStartValues)
                    {
                        startTarget = start_vector;

                        if (StartTargetType == TARGET_TYPE.transform)
                        {
                            if (StartTargetTranform == null)
                            {
                                Debug.LogError("Start target transform is empty");
                                return;
                            }
                            else
                            {
                                if (_rect == null)
                                    startTarget = StartTargetTranform.position;
                                else
                                {
                                    RectTransform otherect = StartTargetTranform.GetComponent<RectTransform>();
                                    if (otherect != null)
                                        startTarget = otherect.anchoredPosition;
                                    else
                                        Debug.LogError("Start target should have a rect tranform if using this script on a UI object or canvas object");
                                }

                            }
                        }

                        if (_rect == null)
                            transform.position = startTarget;
                        else
                            _rect.anchoredPosition = startTarget;
                    }


                    //Animation

                    if (_rect == null)
                    {
                        LeanTween.move(gameObject, Endtarget, _time)
                            .setDelay(_delay)
                            .setEase(_easeType)
                            .setOnStart(() => { OnStart.Invoke(); })
                            .setOnUpdate((Vector3 val) => { OnUpdate_Vector.Invoke(val); })
                            .setOnComplete(() => { OnCompletion.Invoke(); })
                            .destroyOnComplete = DestroyOnCompletion;
                    }
                    else
                    {

                        LeanTween.move(_rect, Endtarget, _time)
                            .setDelay(_delay)
                            .setEase(_easeType)
                            .setOnStart(() => { OnStart.Invoke(); })
                            .setOnUpdate((Vector3 val) => { OnUpdate_Vector.Invoke(val); })
                            .setOnComplete(() => { OnCompletion.Invoke(); })
                            .setIgnoreTimeScale(!PlayInDeltaTime)
                            .destroyOnComplete = DestroyOnCompletion;

                    }

                    break;

                #endregion


                #region Rotate
                case MOTION_TYPE._rotate: // Rotation

                    //Setting END value

                    Endtarget = transform.eulerAngles;

                    if (!DontOverrideEndProperty)
                    {
                        if (EndTargetType == TARGET_TYPE.transform)
                        {
                            if (EndTargetTranform == null)
                            {
                                Debug.LogError("End target transform is empty");
                                return;
                            }
                            else
                                Endtarget = EndTargetTranform.eulerAngles;
                        }
                        else if (EndTargetType == TARGET_TYPE.vector3)
                        {
                            Endtarget = end_vector;
                        }
                    }

                    // Start values
                    if (OverrideStartValues)
                    {
                        startTarget = start_vector;

                        if (StartTargetType == TARGET_TYPE.transform)
                        {
                            if (StartTargetTranform == null)
                            {
                                Debug.LogError("Start target transform is empty");
                                return;
                            }
                            else
                                startTarget = StartTargetTranform.eulerAngles;
                        }

                        transform.eulerAngles = startTarget;
                    }


                    LeanTween.rotate(gameObject, Endtarget, _time)
                        .setDelay(_delay)
                        .setEase(_easeType)
                        .setOnStart(() => { OnStart.Invoke(); })
                        .setOnUpdate((Vector3 val) => { OnUpdate_Vector.Invoke(val); })
                        .setOnComplete(() => { OnCompletion.Invoke(); })
                        .setIgnoreTimeScale(!PlayInDeltaTime)
                        .destroyOnComplete = DestroyOnCompletion;

                    break;
                #endregion


                #region Fade
                case MOTION_TYPE._fade: //Fading

                    if (_rect == null)
                    {
                        if (OverrideStartValues)
                        {
                            LeanTween.alpha(gameObject, startValue, 0f);
                        }

                        //Animation
                        LeanTween.alpha(gameObject, endValue, _time)
                           .setDelay(_delay)
                            .setEase(_easeType)
                            .setOnStart(() => { OnStart.Invoke(); })
                            .setOnUpdate((Vector3 val) => { OnUpdate_Vector.Invoke(val); })
                            .setOnComplete(() => { OnCompletion.Invoke(); })
                            .setIgnoreTimeScale(!PlayInDeltaTime)
                            .destroyOnComplete = DestroyOnCompletion;
                    }
                    else
                    {
                        if (OverrideStartValues)
                        {
                            LeanTween.alpha(_rect, startValue, 0f);
                        }

                        //Animation
                        LeanTween.alpha(_rect, endValue, _time)
                           .setDelay(_delay)
                            .setEase(_easeType)
                            .setOnStart(() => { OnStart.Invoke(); })
                            .setOnUpdate((float val) => { OnUpdate_float.Invoke(val); })
                            .setOnComplete(() => { OnCompletion.Invoke(); })
                            .setIgnoreTimeScale(!PlayInDeltaTime)
                            .destroyOnComplete = DestroyOnCompletion;
                    }

                    break;
                #endregion


                #region Canvas Alpha
                case MOTION_TYPE._fadeCanvasGroup:


                    if (_rect == null || _canvasGroup == null)
                    {
                        Debug.LogError("no rect or canvas group component are found");
                    }
                    else
                    {
                        //Setting END value


                        if (DontOverrideEndProperty)
                        {
                            endValue = _canvasGroup.alpha;
                        }
                        //Setting START value

                        if (OverrideStartValues)
                        {
                            _canvasGroup.alpha = startValue;
                        }



                        LeanTween.alphaCanvas(_canvasGroup, endValue, _time)
                        .setDelay(_delay)
                        .setEase(_easeType)
                        .setOnStart(() => { OnStart.Invoke(); })
                        .setOnUpdate((float val) => { OnUpdate_float.Invoke(val); })
                        .setOnComplete(() => { OnCompletion.Invoke(); })
                        .setIgnoreTimeScale(!PlayInDeltaTime)
                        .destroyOnComplete = DestroyOnCompletion;
                    }
                    break;

                #endregion


                #region Rotationloop

                case MOTION_TYPE._spin:

                    //Setting END value

                    switch (RotationAxis)
                    {
                        case AXIS.Forward:
                            Endtarget = Vector3.forward;
                            break;
                        case AXIS.Backward:
                            Endtarget = Vector3.back;
                            break;
                        case AXIS.Up:
                            Endtarget = Vector3.up;
                            break;
                        case AXIS.Down:
                            Endtarget = Vector3.down;
                            break;
                        case AXIS.Right:
                            Endtarget = Vector3.right;
                            break;
                        case AXIS.Left:
                            Endtarget = Vector3.left;
                            break;
                    }


                    if (OverrideStartValues)
                    {
                        startTarget = start_vector;

                        if (StartTargetType == TARGET_TYPE.transform)
                        {
                            if (StartTargetTranform == null)
                            {
                                Debug.LogError("Start target transform is empty");
                                return;
                            }
                            else
                                startTarget = StartTargetTranform.eulerAngles;
                        }

                        transform.eulerAngles = startTarget;
                    }


                    LeanTween.rotateAround(gameObject, Endtarget, RotationPerSec * 60, _time)
                       .setDelay(_delay)
                        .setEase(_easeType)
                        .setOnStart(() => { OnStart.Invoke(); })
                        .setOnUpdate((Vector3 val) => { OnUpdate_Vector.Invoke(val); })
                        .setOnComplete(() => { OnCompletion.Invoke(); })
                        .setIgnoreTimeScale(!PlayInDeltaTime)
                        .destroyOnComplete = DestroyOnCompletion;

                    break;

                    #endregion

            }

        }




    }
}
