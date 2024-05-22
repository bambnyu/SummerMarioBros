using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb; // R�f�rence au Rigidbody2D
    private Animator anim; // R�f�rence � l'Animator
    private GameObject playerSpawn; // R�f�rence au point de spawn du joueur
    private GameObject pauseMenu;   // R�f�rence au menu pause

    public float speed; // Vitesse de d�placement   
    public float jumpHeight = 6f; // Hauteur d�sir�e pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity; // Gravit� calcul�e
    private float jumpVelocity; // V�locit� de saut calcul�e

    private float Move; // Stocke la valeur de l'axe horizontal
    private bool isFacingRight; // Stocke la direction du joueur
    private GroundedTest groundedTester; // R�f�rence au script GroundedTest
    private bool jump = false; // Stocke l'�tat du saut
    private bool isMovable = true; // Stocke l'�tat du mouvement

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn"); // R�cup�re le GameObject avec le tag "PlayerSpawn"
        //On passe par un parent actif pour atteindre un enfant inactif (FindGameObjectWithTag ne peut pas le faire)
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").transform.GetChild(0).gameObject; // R�cup�re le GameObject avec le tag "PauseMenu"

        // Calcul de la gravit� et de la v�locit� de saut bas�es sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // R�cup�re le script GroundedTest attach� � l'enfant du joueur
    }

    void Update()
    {
        //Ouverture du menu pause
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowPauseMenu();
        }

        if (!isMovable) return; // Si le joueur ne peut pas bouger, on sort de la fonction

        Move = Input.GetAxis("Horizontal");

        DataToStore.instance.UpdatePlayerMovement(Move);

        if (groundedTester.isGrounded) {  //Si notre personnage est au sol alors
            UpdatePlayerSpawn();          // Met � jour le point de spawn du joueur

            if (Input.GetKey(KeyCode.Space)) {  //Si notre joueur a la touche espace enfonc�e
                jump = true;                    // Saut
                DataToStore.instance.jumpingData();
            }
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0); // Animation de course
        anim.SetBool("isJumping", !groundedTester.isGrounded); // Animation de saut

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si n�cessaire
        }
    }

    void FixedUpdate()
    {
        Vector2 ProcessedVelocity = new Vector2(Move * speed, rb.velocity.y);
        predictPlayerPosition(rb.position, ProcessedVelocity);

        //rb.velocity = new Vector2(Move * speed, rb.velocity.y); // D�placement horizontal
        
        if (jump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity); // Saut
            jump = false;
        }

        // Applique la gravit� calcul�e lorsque le joueur n'est pas au sol
        if (!groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.fixedDeltaTime); // Applique la gravit�
        }
    }

    public void ShowPauseMenu()
    {
        //A minima, d�sactiver les mouvements du joueur
        isMovable = !isMovable;
        //Afficher le menu pause
        if (!isMovable)
        {
            pauseMenu.gameObject.SetActive(true);
            Move = 0;
            rb.velocity = new Vector2(0, 0);
            rb.simulated = false;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            rb.velocity = new Vector2(0, 0);
            rb.simulated = true;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    //Update le point de respawn du joueur pour qu'il le suive
    void UpdatePlayerSpawn()
    {
        playerSpawn.transform.position = gameObject.transform.position;
    }

    private void predictPlayerPosition(Vector2 position, Vector2 ProcessedVelocity)
    {
        float WorldminBound = DataToStore.instance.LevelCompoCol2D.bounds.min.x;
        float WorldmaxBound = DataToStore.instance.LevelCompoCol2D.bounds.max.x;

        if (position.x + ProcessedVelocity.x * Time.deltaTime < WorldminBound + 1
            || position.x + ProcessedVelocity.x * Time.deltaTime > WorldmaxBound - 1)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Move * speed, rb.velocity.y);
        }
    }
}
