using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarZ
{
    public class Tank : SkeletonMesh
    {
        private WarZGame WZGame;
        private BasicEffect _effect;


        // we'll use this value when making the wheels roll. It's calculated based on 
        // the distance moved.
        Matrix _wheelRollMatrix = Matrix.Identity;

        // how is the tank oriented? We'll calculate this based on the user's input and
        // the heightmap's normals, and then use it when drawing.
        Matrix _orientation = Matrix.Identity;
        Matrix _turretOrientation = Matrix.Identity;
        private Matrix _canonOrientation = Matrix.Identity;

        // The Simple Animation Sample at creators.xna.com explains the technique that 
        // we will be using in order to roll the tanks wheels. In this technique, we
        // will keep track of the ModelBones that control the wheels, and will manually
        // set their transforms. These next eight fields will be used for this
        // technique.
        Matrix[] _boneTransforms;
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;

        ModelBone turretBone;
        ModelBone canonBone;

        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;

        Matrix turretTransform;
        Matrix canonTransform;


        private bool isAiEnemy;
        private bool isGhost;

        private Vector3 _specularColor_original;
        private Vector3 _specularColor_red;

        public Tank(WarZGame game, Model model)
            : base(game, model)
        {
            WZGame = game;

            // _scale /= 300;
            _model.Root.Transform *= Matrix.CreateScale(Vector3.One / 300);
            _model.Root.Transform *= Matrix.CreateRotationY(MathHelper.Pi);

            _boneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(_boneTransforms);

            LoadBoundingSphere();


            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = model.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = model.Bones["r_front_wheel_geo"];
            turretBone = model.Bones["turret_geo"];
            canonBone = model.Bones["canon_geo"];

            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;

            turretTransform = turretBone.Transform;
            canonTransform = turretBone.Transform;


            _specularColor_original = ((BasicEffect)(_model.Meshes[0].Effects[0])).SpecularColor;
            _specularColor_red = Color.Red.ToVector3();

        }

        #region Shadows
        /*
        private static DepthFormat SelectStencilMode()
        {
            // Check stencil formats
            GraphicsAdapter adapter = GraphicsAdapter.DefaultAdapter;
            

            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format,
                format, DepthFormat.Depth24Stencil8))
                return DepthFormat.Depth24Stencil8;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format,
                format, DepthFormat.Depth24Stencil8Single))
                return DepthFormat.Depth24Stencil8Single;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format,
                format, DepthFormat.Depth24Stencil4))
                return DepthFormat.Depth24Stencil4;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format,
                format, DepthFormat.Depth15Stencil1))
                return DepthFormat.Depth15Stencil1;
            else
                throw new InvalidOperationException(
                    "Could Not Find Stencil Buffer for Default Adapter");
        }
         * */
        #endregion


        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        public override void DrawModel(Camera camera)
        {
            Prepare3DRenderer.PrepareGraphicsDevice();
            // Set the world matrix as the root transform of the model.
            //_model.Root.Transform = Matrix.CreateScale(_scale) * _orientation * Matrix.CreateTranslation(Position);

            leftBackWheelBone.Transform = _wheelRollMatrix * leftBackWheelTransform;
            rightBackWheelBone.Transform = _wheelRollMatrix * rightBackWheelTransform;
            leftFrontWheelBone.Transform = _wheelRollMatrix * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = _wheelRollMatrix * rightFrontWheelTransform;
            _model.CopyAbsoluteBoneTransformsTo(_boneTransforms);

            Matrix worldMatrix = _orientation * Matrix.CreateTranslation(Position);
            Matrix turretMatrix = _turretOrientation * Matrix.CreateTranslation(Position);
            Matrix canonMatrix = Matrix.CreateScale(_scale) * _canonOrientation * Matrix.CreateTranslation(Position);

            _model.CopyAbsoluteBoneTransformsTo(_boneTransforms);


            WZGame.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    switch (mesh.Name)
                    {
                        case "canon_geo":
                            //effect.World = _boneTransforms[mesh.ParentBone.Index] * canonMatrix;
                            effect.World = _boneTransforms[mesh.ParentBone.Index] * turretMatrix;
                            break;
                        case "hatch_geo":
                        case "turret_geo":
                            effect.World = _boneTransforms[mesh.ParentBone.Index] * turretMatrix;
                            break;

                        default:
                            effect.World = _boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                            break;
                    }

                    //effect.World = _boneTransforms[mesh.ParentBone.Index];
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    
                    if (IsGhost)
                        effect.Alpha = 0.5f;
                    else
                       effect.Alpha = 1f;




                    if (isAiEnemy)
                        effect.SpecularColor = _specularColor_red;
                    else
                        effect.SpecularColor = _specularColor_original;
                }
                mesh.Draw();
            }
            WZGame.GraphicsDevice.BlendState = BlendState.Opaque;
        }

        void LoadBoundingSphere()
        {
            BoundingSphere completeBoundingSphere = new BoundingSphere();
            foreach (ModelMesh mesh in _model.Meshes)
            {
                BoundingSphere origMeshSphere = mesh.BoundingSphere;
                BoundingSphere transMeshSphere = XNAUtils.TransformBoundingSphere(origMeshSphere, _boneTransforms[mesh.ParentBone.Index]);
                completeBoundingSphere = BoundingSphere.CreateMerged(completeBoundingSphere, transMeshSphere);
            }
            _model.Tag = completeBoundingSphere;
        }




        #region Properties
        public Matrix World
        {
            get { return Matrix.CreateScale(_scale) * _orientation * Matrix.CreateTranslation(Position); }
        }

        public Matrix WheelRollMatrix
        {
            get { return _wheelRollMatrix; }
            set { _wheelRollMatrix = value; }
        }


        public Matrix Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public Matrix TurretOrientation
        {
            get { return _turretOrientation; }
            set { _turretOrientation = value; }
        }

        public Matrix CanonOrientation
        {
            get { return _canonOrientation; }
            set { _canonOrientation = value; }
        }

        public bool IsAi
        {
            get { return isAiEnemy; }
            set { isAiEnemy = value; }
        }

        public bool IsGhost
        {
            get { return isGhost; }
            set { isGhost = value; }
        }

        #endregion

    }
}
