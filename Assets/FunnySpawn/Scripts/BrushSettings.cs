using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miles {

    public class BrushSettings {
        public Vector3 origin;
        public Vector3 target;
        public Vector3 direction;
        public float heightOffset;
        public float angleOffset;

        Vector3 eulerAngles;
        public Vector3 EulerAngles {
            get {
                return eulerAngles;
            }
        }
        private Quaternion final_Quaternion;
        public Quaternion Final_Quaternion {
            get {
                return final_Quaternion;
            }
        }
        public bool use_WorldY;
        public bool use_AlignedDirection;
        public bool use_FixedRotation;
        public bool use_RandomScale;
        public float unitformScaleOffset;
        public float scale_Min;
        public float scale_Max;
        public float radius;
        public float radiusScaler;
        public Color color;
        public string label;
        public BrushSettings() {
            this.origin = Vector3.zero;
            this.target = Vector3.zero;
            this.direction = Vector3.up;
            this.radius = 1.0f;
            this.color = Color.yellow;
            this.label = "";
            this.radiusScaler = 1.0f;
            this.heightOffset = 0.0f;
            this.angleOffset = 0.0f;
            this.eulerAngles = Vector3.zero;
            this.use_AlignedDirection = false;
            this.use_WorldY = true;
            this.use_FixedRotation = true;
            this.use_RandomScale = false;
            this.scale_Min = 1.0f;
            this.scale_Max = 1.0f;
            this.unitformScaleOffset = 1.0f;

        }

        public Matrix4x4 GetTransform() {
            Matrix4x4 translate = Matrix4x4.Translate(target);

            Matrix4x4 rotation1;
            Matrix4x4 rotation2;
            rotation1 = Matrix4x4.Rotate(Quaternion.AngleAxis(angleOffset, direction));
            if (use_AlignedDirection) {
                rotation2 = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.up, direction));
            } else {
                rotation2 = Matrix4x4.identity;
            }
            Matrix4x4 rotation_Final = rotation1 * rotation2;
            this.final_Quaternion = rotation_Final.rotation;

            this.eulerAngles = rotation_Final.rotation.eulerAngles;

            Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * unitformScaleOffset);
            return translate * rotation_Final * scale;
        }
    }
}
