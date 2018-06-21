using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour {
    public Camera camera_camera;
    public GrapplingHookMode grappling_hook_mode;
    public bool draw_dev_line = true;
    public float break_tether_velo;

    public enum GrapplingHookMode {Ratcheting, Loose, Rigid};

    private Rigidbody rigidbody;
    private GameObject player_camera;

    private GameObject hooked_node;
    private float node_distance;
    private float start_node_distance;
    private GameObject grappling_line;

    // Use this for initialization
    void Start() {
        this.rigidbody = gameObject.GetComponent<Rigidbody>();

        if (draw_dev_line)
        {
            grappling_line = new GameObject("GrapplingLine");
            grappling_line.SetActive(false);
            LineRenderer line_renderer = grappling_line.AddComponent<LineRenderer>();
            line_renderer.endWidth = .1f;
            line_renderer.startWidth = .01f;
        }
        hooked_node = null;
        return;
	}

    // Update is called once per frame
    void Update()
    {
        //hooked node is null and player has pressed mouse
        if (Input.GetMouseButtonDown(0) && hooked_node == null)
        {
            Ray ray = this.camera_camera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

            RaycastHit raycast_hit;
            if (Physics.Raycast(ray, out raycast_hit) && (raycast_hit.transform.tag == "grappling_node"))
            {
                hooked_node = raycast_hit.collider.gameObject;
                node_distance = start_node_distance = (Vector3.Distance(hooked_node.transform.position, gameObject.transform.position));

                if(draw_dev_line)
                {
                    grappling_line.SetActive(true);
                }

            }

            return;
        }

        //hook is attached and player let go of mouse
        if (Input.GetMouseButtonUp(0) && hooked_node != null)
        {
            BreakTether();
        }

        return;
    }

    void FixedUpdate()
    {
        //hook is attached and player held down mouse
        if (hooked_node != null && Input.GetMouseButton(0))
        {
            //if the velo is > break_tether_velo, break tether
            if (rigidbody.velocity.magnitude > break_tether_velo)
            {
                BreakTether();
                return;
            }

            if (draw_dev_line)
            {
                Vector3[] line_vortex_arr = { gameObject.transform.position, hooked_node.transform.position };
                grappling_line.GetComponent<LineRenderer>().SetPositions(line_vortex_arr);
            }

            //gets velocity in units/frame, then gets the position for next frame
            Vector3 curr_velo_upf = rigidbody.velocity * Time.fixedDeltaTime;
            Vector3 test_pos = gameObject.transform.position + curr_velo_upf;

            //Depending on the mode, ApplyTensionForce will be called under certain conditions
            if (grappling_hook_mode == GrapplingHookMode.Ratcheting)
            {
                if (Vector3.Distance(test_pos, hooked_node.transform.position) < node_distance)
                {
                    node_distance = Vector3.Distance(test_pos, hooked_node.transform.position);
                }
                else
                {
                    ApplyTensionForce(curr_velo_upf, test_pos);
                }
            }
            else if(grappling_hook_mode == GrapplingHookMode.Loose)
            {
                if(Vector3.Distance(test_pos, hooked_node.transform.position) > node_distance)
                {
                    ApplyTensionForce(curr_velo_upf, test_pos);
                }
            }
            else
            {
                ApplyTensionForce(curr_velo_upf, test_pos);
            }
        }

        return;
    }

    private void ApplyTensionForce(Vector3 curr_velo_upf, Vector3 test_pos)
    {
        //finds what the new velocity is due to tension force grappling hook
        //normalized vector that from node to test pos
        Vector3 node_to_test = (test_pos - hooked_node.transform.position).normalized;
        Vector3 new_pos = (node_to_test * node_distance) + hooked_node.transform.position;
        Vector3 new_velocity = new_pos - gameObject.transform.position;

        //force_tension = mass * (d_velo / d_time)
        //where d_velo is new_velocity - old_velocity
        Vector3 delta_velocity = new_velocity - curr_velo_upf;
        Vector3 tension_force = (rigidbody.mass * (delta_velocity / Time.fixedDeltaTime));

        rigidbody.AddForce(tension_force);
    }

    public void BreakTether()
    {
        hooked_node = null;
        if(draw_dev_line)
        {
            grappling_line.SetActive(false);
        }
        return;
    }

}
