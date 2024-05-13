using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb; // R�f�rence au Rigidbody2D
    private Animator anim; // R�f�rence � l'Animator

    public float speed; // Vitesse de d�placement   
    public float jumpHeight = 6f; // Hauteur d�sir�e pour le saut
    public float timeToJumpApex = 0.6f; // Temps pour atteindre le point le plus haut du saut

    private float gravity; // Gravit� calcul�e
    private float jumpVelocity; // V�locit� de saut calcul�e

    private float Move; // Stocke la valeur de l'axe horizontal
    private bool isFacingRight; // Stocke la direction du joueur
    private GroundedTest groundedTester; // R�f�rence au script GroundedTest

    void Start()
    {
        isFacingRight = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Calcul de la gravit� et de la v�locit� de saut bas�es sur la hauteur de saut et le temps pour atteindre l'apex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        groundedTester = GetComponentInChildren<GroundedTest>(); // R�cup�re le script GroundedTest attach� � l'enfant du joueur
    }

    void Update()
    {
        Move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(Move * speed, rb.velocity.y); // D�placement horizontal

        if (Input.GetKeyDown(KeyCode.Space) && groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity); // Saut
        }

        anim.SetBool("isRunning", Mathf.Abs(Move) > 0); // Animation de course

        if ((!isFacingRight && Move > 0) || (isFacingRight && Move < 0))
        {
            Flip(); // Retourne le joueur si n�cessaire
        }
    }

    void FixedUpdate()
    {
        // Applique la gravit� calcul�e lorsque le joueur n'est pas au sol
        if (!groundedTester.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + gravity * Time.fixedDeltaTime); // Applique la gravit�
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
