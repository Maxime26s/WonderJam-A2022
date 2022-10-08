using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField]
    private Animation shotgunAnimation = null;
    [SerializeField]
    private Animation shotgunRecoilAnimation = null;
    [SerializeField]
    private CharacterController characterController = null;
    [SerializeField]
    private Transform playerTransform = null;

    [SerializeField]
    private ParticleSystem muzzleFlash = null;
    [SerializeField]
    private ParticleSystem gunSmoke = null;
    [SerializeField]
    private ShellSpawner ejectShell = null;
    [SerializeField]
    private float weaponRange = 10f;
    [SerializeField]
    private LayerMask layerMask = new LayerMask();
    [SerializeField]
    private int bulletsPerShot = 10;
    [SerializeField]
    private float spreadFactor = 10f;
    [SerializeField]
    private float heatPerShot = 0.4f;
    [SerializeField]
    private float heatDownSpeed = 2f;

    [SerializeField]
    private GameObject metalHitEffect;
    [SerializeField]
    private GameObject sandHitEffect;
    [SerializeField]
    private GameObject stoneHitEffect;
    [SerializeField]
    private GameObject waterLeakEffect;
    [SerializeField]
    private GameObject waterLeakExtinguishEffect;
    [SerializeField]
    private GameObject[] fleshHitEffects;
    [SerializeField]
    private GameObject woodHitEffect;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip shotgunBlastAudio;

    [SerializeField]
    private bool onCooldown = false;

    [SerializeField]
    private float heatValue = 0f;
    [SerializeField]
    private float shotgunBulletForce = 1000f;
    [SerializeField]
    private float shotgunTeleportBackDistance = 5f;


    void Update()
    {
        if (InputManager.Instance.PlayerGetFireInput() && !onCooldown) {
            Shoot();
        }
    }

    private void FixedUpdate() {
        SmokeColor();
    }

    private void Shoot() {
        shotgunAnimation.Play();
        shotgunRecoilAnimation.Play();
        muzzleFlash.Play();
        audioSource.PlayOneShot(shotgunBlastAudio);

        HeatUp();

        TeleportPlayer();

        onCooldown = true;

        RaycastHit hit;
        for (int i = 0; i < bulletsPerShot; i++) {
            Vector3 randomVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Vector3 shotDirection = Camera.main.transform.forward + randomVector * Random.Range(0f, spreadFactor);
            if (Physics.Raycast(Camera.main.transform.position, shotDirection, out hit, weaponRange, layerMask)) {
                HandleHit(hit);
            }
        }
    }

    private void TeleportPlayer()
    {
        Vector3 direction = Camera.main.transform.forward;
        direction.y = 0;
        direction = -direction.normalized;
        characterController.enabled = false;
        playerTransform.position += (direction * shotgunTeleportBackDistance);
        characterController.enabled = true;
    }

    private void HeatUp() {
        heatValue += heatPerShot;
        heatValue = Mathf.Clamp(heatValue, 0f, 1f);
    }

    private void SmokeColor() {
        Color smokeColor = gunSmoke.main.startColor.color;
        var main = gunSmoke.main;
        smokeColor.a = heatValue;
        main.startColor = smokeColor;
        heatValue -= Time.deltaTime * heatDownSpeed;
        heatValue = Mathf.Clamp(heatValue, 0f, 1f);
    }

    void HandleHit(RaycastHit hit) {

        CheckRigidbody(hit);

        if (hit.collider.sharedMaterial != null) {
            string materialName = hit.collider.sharedMaterial.name;

            switch (materialName) {
                case "Metal":
                    SpawnDecal(hit, metalHitEffect);
                    break;
                case "Sand":
                    SpawnDecal(hit, sandHitEffect);
                    break;
                case "Stone":
                    SpawnDecal(hit, stoneHitEffect);
                    break;
                case "WaterFilled":
                    SpawnDecal(hit, waterLeakEffect);
                    SpawnDecal(hit, metalHitEffect);
                    break;
                case "Wood":
                    SpawnDecal(hit, woodHitEffect);
                    break;
                case "Meat":
                    SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
                    break;
                case "Character":
                    SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
                    break;
                case "WaterFilledExtinguish":
                    SpawnDecal(hit, waterLeakExtinguishEffect);
                    SpawnDecal(hit, metalHitEffect);
                    break;
            }
        } else {
            SpawnDecal(hit, metalHitEffect);
        }
    }

    private void CheckRigidbody(RaycastHit hit) {
        Rigidbody rigidbody = hit.collider.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null) {
            rigidbody.AddForce((hit.point - Camera.main.transform.position) * shotgunBulletForce);
        }
    }

    void SpawnDecal(RaycastHit hit, GameObject prefab) {
        GameObject spawnedDecal = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
        spawnedDecal.transform.SetParent(hit.collider.transform);
    }

    public void EjectShell() {
        ejectShell.SpawnShell();
    }

}
