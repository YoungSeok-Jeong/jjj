using UnityEngine;
using System.Collections;
using Spine.Unity;
using Replay;

public class Actor : MonoBehaviour {
    /*
     * 오직 리플레이를 위한 스크립트
     * 스파인 애니메이션과 콜백 함수만 참고하여 비주얼 컨트롤
     */
         
    private SkeletonAnimation m_skeletonAnimation;

    // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
    private Spine.AnimationState m_spineAnimationState;
    private Spine.Skeleton m_skeleton;

    public delegate void DelegateAct();
    DelegateAct m_myDelegate;

    public void Awake() {
        this.m_skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (this.m_skeletonAnimation != null) {
            this.m_spineAnimationState = m_skeletonAnimation.AnimationState;
            this.m_skeleton = m_skeletonAnimation.Skeleton;
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Performance(ReplayResult replay) {
        this.m_myDelegate = replay.DonePlaying;
        ReplayAction action = replay.GetAction();
        JLog.Log(JLog.LogCategory.Replay, "{0}", replay.ToString());
        switch (action) {
            case ReplayAction.Attack:
                StartCoroutine(DoAttack(replay.GetStartTime(), "atk"));
                break;
            case ReplayAction.Damaged:
                if (replay.isDead()) {
                    StartCoroutine(DoDie(replay.GetStartTime()));
                } else {
                    StartCoroutine(DoDamaged(replay.GetStartTime()));
                }
                break;
            case ReplayAction.Defense:
                StartCoroutine(DoDefense(replay.GetStartTime()));
                break;
            case ReplayAction.Skill:
                StartCoroutine(DoAttack(replay.GetStartTime(), "ready_skill_3"));
                break;
            default:
                this.m_myDelegate();
                break;
        }
    }

    public IEnumerator DoAttack(float delay, string attackAni) {
        yield return this.WaitForSeconds(delay);

        Vector3 orgPos = this.transform.localPosition;
        Vector3 targetPos = this.transform.localPosition + Vector3.forward;
        yield return this.MoveTo(targetPos, 4.0f);

        yield return PlayAndWaitAnimation(attackAni, "idle");
        yield return this.MoveTo(orgPos, 4.0f);
        this.m_myDelegate();
    }

    public IEnumerator DoDefense(float delay) {
        yield return this.WaitForSeconds(delay);

        Vector3 orgPos = this.transform.localPosition;
        Vector3 targetPos = this.transform.localPosition + Vector3.forward;
        yield return this.MoveTo(targetPos, 4.0f);

        yield return PlayAndWaitAnimation("ready_skill_2", "idle");
        yield return this.MoveTo(orgPos, 4.0f);
        this.m_myDelegate();
    }

    public IEnumerator DoDamaged(float delay) {
        yield return this.WaitForSeconds(delay);

        yield return PlayAndWaitAnimation("hitted", "idle");
        this.m_myDelegate();
    }

    public IEnumerator DoDie(float delay) {
        yield return this.WaitForSeconds(delay);

        yield return PlayAnimation("dying_bound", false);
        this.m_myDelegate();
    }

    public IEnumerator WaitForSeconds(float delay) {
        float timer = 0.0f;
        while (timer < delay) {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator MoveTo(Vector3 targetPos, float speed) {
        float dist = (targetPos - this.transform.localPosition).sqrMagnitude;

        while (dist > float.Epsilon) {
            Vector3 newPos = Vector3.MoveTowards(this.transform.localPosition, targetPos, speed * Time.deltaTime);
            this.transform.localPosition = newPos;
            dist = (targetPos - this.transform.localPosition).sqrMagnitude;
            yield return null;
        }
    }

    public IEnumerator PlayAndWaitAnimation(string aniName, string nextAni) {
        this.m_spineAnimationState.SetAnimation(0, aniName, false);
        while (!m_spineAnimationState.GetCurrent(0).IsComplete) {
            yield return null;
        }

        this.m_spineAnimationState.SetAnimation(0, nextAni, true);
    }

    public IEnumerator PlayAnimation(string aniName, bool loop) {
        this.m_spineAnimationState.SetAnimation(0, aniName, loop);
        while (!m_spineAnimationState.GetCurrent(0).IsComplete) {
            yield return null;
        }
    }
}
