using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WarZ
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Terrain : Microsoft.Xna.Framework.DrawableGameComponent, IGotCamera
    {
        private BasicEffect _effect;
        private Camera _camera;



        private Texture2D _texture;

        private Texture2D _heightMap;
        private float[,] _heightData;
        private int _width;
        private int _height;
        private float _heightScale = 20.0f;

        private RasterizerState wireframeRasterizer;
        private RasterizerState solidRasterizer;
        private RasterizerState actualRasterizer;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private bool _showNormals;

        private VertexPositionColor[] _normalsList;

        private VertexPositionNormalTexture[] _terrainVertices;

        public Terrain(Game game, Camera camera)
            : base(game)
        {
            _camera = camera;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            wireframeRasterizer = new RasterizerState();
            //wireframeRasterizer.CullMode = CullMode.None;
            wireframeRasterizer.FillMode = FillMode.WireFrame;

            solidRasterizer = new RasterizerState();
            // solidRasterizer.CullMode = CullMode.None;
            solidRasterizer.FillMode = FillMode.Solid;
            //solidRasterizer.MultiSampleAntiAlias = true;

            actualRasterizer = solidRasterizer;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _effect = new BasicEffect(GraphicsDevice);


            _texture = Game.Content.Load<Texture2D>(@"Textures\grass");
            _heightMap = Game.Content.Load<Texture2D>(@"Textures\heightmap");
            LoadHeightData(_heightMap);

            _terrainVertices = CreateTerrainVertices();
            ushort[] terrainIndices = CreateTerrainIndices();
            // terrainVertices = GenerateNormalsForTriangleStrip(terrainVertices, terrainIndices);

            GenerateTerrainNormals_2(ref _terrainVertices, terrainIndices);

            fillNormalVectorList(out _normalsList, ref _terrainVertices);

            CreateBuffers(_terrainVertices, terrainIndices);
            base.LoadContent();
        }
        public bool LimitsCollision(Vector3 position)
        {
            return position.X < 0.1f || position.Z > -0.1f ||
                   position.Z < -Width + 2 || position.X > Height - 2;


        }
        public bool LimitsCollisionCheckWithReposition(IComponent3D component)
        {
            bool collided = false;

            if (component.Position.X < 0.1f)
            {
                Vector3 newPosition = component.Position;
                newPosition.X = 0.1f;
                component.Position = newPosition;

                collided = true;
            }

            if (component.Position.Z > -0.1f)
            {
                Vector3 newPosition = component.Position;
                newPosition.Z = -0.1f;
                component.Position = newPosition;
                collided = true;

            }

            if (component.Position.Z < -Width + 2)
            {
                Vector3 newPosition = component.Position;
                newPosition.Z = -Width + 2;
                component.Position = newPosition;
                collided = true;

            }

            if (component.Position.X > Height - 2)
            {
                Vector3 newPosition = component.Position;
                newPosition.X = Height - 2;
                component.Position = newPosition;
                collided = true;
            }

            return collided;
        }

        #region Terrain Vertices Generation

        private VertexPositionNormalTexture[] CreateTerrainVertices()
        {
            _width = _heightData.GetLength(0);
            _height = _heightData.GetLength(1);
            VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[_width * _height];
            int i = 0;
            for (int z = 0; z < _height; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Vector3 position = new Vector3(x, _heightData[x, z], -z); //-z
                    Vector3 normal = Vector3.Zero;
                    Vector2 texCoord = new Vector2((float)x / 20.0f, (float)z / 20.0f);
                    terrainVertices[i++] = new VertexPositionNormalTexture(position, normal, texCoord);
                }
            }


            //GenerateTerrainNormals(terrainVertices);


            return terrainVertices;
        }

        private void GenerateTerrainNormals_2(ref VertexPositionNormalTexture[] vertices, ushort[] indices)
        {
            bool swappedWinding = false;
            int k = 0;
            ushort ind0, ind1, ind2 = 0;

            for (int i = 2; i < indices.Length; i++)
            {
                ind0 = indices[i];
                ind1 = indices[i - 1];
                ind2 = indices[i - 2];

                Vector3 firstVec = vertices[ind1].Position - vertices[indices[i]].Position;
                Vector3 secondVec = vertices[ind2].Position - vertices[indices[i]].Position;
                Vector3 normal = Vector3.Cross(firstVec, secondVec);
                normal.Normalize();

                if (swappedWinding)
                    normal *= -1;

                if (!float.IsNaN(normal.X))
                {
                    //add the calculated normal
                    vertices[ind0].Normal += normal;
                    vertices[ind1].Normal += normal;
                    vertices[ind2].Normal += normal;

                    //normalization
                    vertices[ind0].Normal.Normalize();
                    vertices[ind1].Normal.Normalize();
                    vertices[ind2].Normal.Normalize();
                }

                swappedWinding = !swappedWinding;
            }




        }

        /// <summary>
        /// 
        /// Requires: _width _height to be prev set.
        /// </summary>
        /// <param name="normalList"></param>
        /// <param name="vertices"></param>
        private void fillNormalVectorList(out VertexPositionColor[] normalList, ref VertexPositionNormalTexture[] vertices)
        {
            // * 2 because you always need the origin of the vector
            normalList = new VertexPositionColor[_width * _height * 2];
            int k = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                //DEBUG: this will be used to draw the normals in the terrain
                Vector3 position = vertices[i].Position;
                Vector3 normal = vertices[i].Normal;
                normalList[k++] = new VertexPositionColor(position, Color.White);
                normalList[k++] = new VertexPositionColor(position + normal, Color.White);
            }

        }

        /// <summary>
        /// Lazy Normal generation
        /// The normals of the terrain perimeter will be Vector3.Up
        /// </summary>
        /// <param name="terrainVertices"></param>
        private void GenerateTerrainNormals(VertexPositionNormalTexture[] terrainVertices)
        {
            int i = 0;
            int k = 0;
            for (int z = 1; z < _height - 1; z++)
            {

                for (int x = 1; x < _width - 1; x++)
                {

                    Vector3 center = terrainVertices[x + z * _width].Position;

                    Vector3 north = Vector3.Subtract(terrainVertices[x + (z - 1) * _width].Position, center);
                    Vector3 east = Vector3.Subtract(terrainVertices[(x - 1) + z * _width].Position, center);
                    Vector3 south = Vector3.Subtract(terrainVertices[x + (z + 1) * _width].Position, center);
                    Vector3 west = Vector3.Subtract(terrainVertices[(x + 1) + z * _width].Position, center);


                    //Get vector positions
                    Vector3 northVector = terrainVertices[x + (z - 1) * _width].Position;
                    Vector3 eastVector = terrainVertices[(x - 1) + z * _width].Position;
                    Vector3 southVector = terrainVertices[x + (z + 1) * _width].Position;
                    Vector3 westVector = terrainVertices[(x + 1) + z * _width].Position;


                    /*
                    //Get vector positions
                    Vector3 northVector = terrainVertices[x + (z + 1) * _width].Position;
                    Vector3 eastVector = terrainVertices[(x - 1) + (-z) * _width].Position;
                    Vector3 southVector = terrainVertices[x + (z - 1) * _width].Position;
                    Vector3 westVector = terrainVertices[(x + 1) + (-z) * _width].Position;
                    */

                    //fazer multiplicacao counter clockwise usando o Cross
                    Vector3 normalNorth = Vector3.Cross(northVector, eastVector);
                    Vector3 normalEast = Vector3.Cross(eastVector, southVector);
                    Vector3 normalSouth = Vector3.Cross(southVector, westVector);
                    Vector3 normalWest = Vector3.Cross(westVector, northVector);


                    //Vector normalization
                    northVector.Normalize();
                    eastVector.Normalize();
                    southVector.Normalize();
                    westVector.Normalize();


                    Vector3 normal = (normalNorth + normalEast + normalSouth + normalWest) / 4;


                    _normalsList[k++] = new VertexPositionColor(center, Color.White);
                    _normalsList[k++] = new VertexPositionColor(center - normal, Color.White);

                    //Means that the vertice is not in the terrain perimeter.
                    terrainVertices[i++].Normal = normal;
                }
            }
        }

        private ushort[] CreateTerrainIndices()
        {
            int _width = _heightData.GetLength(0);
            int _height = _heightData.GetLength(1);
            ushort[] terrainIndices = new ushort[(_width) * 2 * (_height - 1)];

            ushort i = 0;
            ushort z = 0;
            while (z < _height - 1)
            {
                for (int x = 0; x < _width; x++)
                {
                    terrainIndices[i++] = (ushort)(x + z * _width);
                    terrainIndices[i++] = (ushort)(x + (z + 1) * _width);
                }
                z++;
                if (z < _height - 1)
                {
                    for (int x = _width - 1; x >= 0; x--)
                    {
                        terrainIndices[i++] = (ushort)(x + (z + 1) * _width);
                        terrainIndices[i++] = (ushort)(x + z * _width);
                    }
                }
                z++;
            }
            return terrainIndices;
        }

        private void CreateBuffers(VertexPositionNormalTexture[] vertices, ushort[] indices)
        {
            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);

            _indexBuffer = new IndexBuffer(GraphicsDevice, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        private void LoadHeightData(Texture2D _heightMap)
        {
            float minimum_height = 200;
            float maximum_height = 0;
            int _width = _heightMap.Width;
            int _height = _heightMap.Height;
            Color[] _heightMapColors = new Color[_width * _height];

            _heightMap.GetData<Color>(_heightMapColors);

            _heightData = new float[_width, _height];

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    _heightData[x, y] = _heightMapColors[x + y * _width].R;
                    if (_heightData[x, y] < minimum_height) minimum_height = _heightData[x, y];
                    if (_heightData[x, y] > maximum_height) maximum_height = _heightData[x, y];
                }

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    _heightData[x, y] = (_heightData[x, y] - minimum_height) / (maximum_height - minimum_height) * _heightScale;
        }
        #endregion

        #region Terrain Height Methods
        public float GetExactHeightAt(float x, float z)
        {
            z *= -1;

            bool invalid = x < 0;
            invalid |= z < 0;
            invalid |= x >= _heightData.GetLength(0) - 1;
            invalid |= z >= _heightData.GetLength(1) - 1;
            if (invalid)
                return 10;



            return _heightData[(int)x, (int)z];
        }

        public float GetInterpolatedHeightAt(float x, float z)
        {
            z *= -1;

            bool invalid = x < 0;
            invalid |= z < 0;
            invalid |= x >= _heightData.GetLength(0) - 1;
            invalid |= z >= _heightData.GetLength(1) - 1;
            if (invalid)
                return 10;

            int xLower = (int)x;
            int xHigher = xLower + 1;
            float xRelative = (x - xLower) / ((float)xHigher - (float)xLower);

            int zLower = (int)z;
            int zHigher = zLower + 1;
            float zRelative = (z - zLower) / ((float)zHigher - (float)zLower);

            float heightLxLz = _heightData[xLower, zLower];
            float heightLxHz = _heightData[xLower, zHigher];
            float heightHxLz = _heightData[xHigher, zLower];
            float heightHxHz = _heightData[xHigher, zHigher];

            bool cameraAboveLowerTriangle = (xRelative + zRelative < 1);

            float finalHeight;
            if (cameraAboveLowerTriangle)
            {
                finalHeight = heightLxLz;
                finalHeight += zRelative * (heightLxHz - heightLxLz);
                finalHeight += xRelative * (heightHxLz - heightLxLz);
            }
            else
            {
                finalHeight = heightHxHz;
                finalHeight += (1.0f - zRelative) * (heightHxLz - heightHxHz);
                finalHeight += (1.0f - xRelative) * (heightLxHz - heightHxHz);
            }

            return finalHeight;
        }

        public Vector3 GetInterpolatedNormalAt(float x, float z)
        {
            z *= -1;

            bool invalid = x < 0;
            invalid |= z < 0;
            invalid |= x >= _heightData.GetLength(0) - 1;
            invalid |= z >= _heightData.GetLength(1) - 1;
            if (invalid)
                return Vector3.Up;

            int xLower = (int)x;
            int xHigher = xLower + 1;
            float xRelative = (x - xLower) / ((float)xHigher - (float)xLower);

            int zLower = (int)z;
            int zHigher = zLower + 1;
            float zRelative = (z - zLower) / ((float)zHigher - (float)zLower);



            Vector3 normalLxLz = _terrainVertices[xLower + zLower * _width].Normal;
            Vector3 normalLxHz = _terrainVertices[xLower + zHigher * _width].Normal;
            Vector3 normalHxLz = _terrainVertices[xHigher + zLower * _width].Normal;
            Vector3 normalHxHz = _terrainVertices[xHigher + zHigher * _width].Normal;

            bool cameraAboveLowerTriangle = (xRelative + zRelative < 1);

            Vector3 finalNormal;
            if (cameraAboveLowerTriangle)
            {
                finalNormal = normalLxLz;
                finalNormal += zRelative * (normalLxHz - normalLxLz);
                finalNormal += xRelative * (normalHxLz - normalLxLz);
            }
            else
            {
                finalNormal = normalHxHz;
                finalNormal += (1.0f - zRelative) * (normalHxLz - normalHxHz);
                finalNormal += (1.0f - xRelative) * (normalLxHz - normalHxHz);
            }

            return finalNormal;
        }

        public float GetPositionHeight_2(float x, float z)
        {
            z *= -1;
            bool invalid = x < 0;
            invalid |= z < 0;
            invalid |= x > _heightData.GetLength(0) - 1;
            invalid |= z > _heightData.GetLength(1) - 1;

            if (invalid)
                return 10;


            int xA = (int)x;
            int xB = xA + 1;
            int xC = xA;
            int xD = xB;

            int zA = (int)z;
            int zB = zA;
            int zC = zA + 1;
            int zD = zC;

            float yA = _heightData[xA, zA];
            float yB = _heightData[xB, zB];
            float yC = _heightData[xC, zC];
            float yD = _heightData[xD, zD];

            float dA = x - xA;
            float dB = xB - x;
            float dC = zC - z;
            float dD = zD - z;

            float dCD = zC - z;
            float dAB = z - zA;

            float yAB = (yA * dB + yB * dA) / (dA + dB);
            float yCD = (yC * dD + yA * dC) / (dD + dC);

            float y = yAB * dCD + yCD * dAB;

            float yHammer = _heightData[(int)x, (int)z];


            return y;

            return _heightData[(int)x, (int)z];
        }
        public float GetPositionHeight(float x, float z)
        {
            return GetInterpolatedHeightAt(x, z);
            return GetPositionHeight_2(x, z);
        }
        #endregion

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            if (KeyboardHelper.IsKeyPressed(Keys.F)) //Change fill mode
                actualRasterizer = actualRasterizer == solidRasterizer ? wireframeRasterizer : solidRasterizer;
            if (KeyboardHelper.IsKeyPressed(Keys.N))
                _showNormals = !_showNormals;

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            Prepare3DRenderer.PrepareGraphicsDevice();

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            GraphicsDevice.RasterizerState = actualRasterizer;
            _effect.World = Matrix.Identity;
            _effect.View = _camera.View;
            _effect.Projection = _camera.Projection;


            _effect.EnableDefaultLighting();
            _effect.DirectionalLight0.Direction = new Vector3(1, -1, 1);
            _effect.DirectionalLight0.Enabled = true;
            _effect.AmbientLightColor = new Vector3(0.3f, 0.3f, 0.3f);
            _effect.DirectionalLight1.Enabled = false;
            _effect.DirectionalLight2.Enabled = false;
            _effect.SpecularColor = new Vector3(0, 0, 0);


            _effect.PreferPerPixelLighting = false;

            _effect.Texture = _texture;
            _effect.TextureEnabled = true;

            _effect.FogEnabled = false;
            _effect.FogColor = new Vector3(1, 0, 0);
            _effect.FogStart = 1;
            _effect.FogEnd = 250;
            

            if (_showNormals)
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _normalsList, 0, _normalsList.Length / 2);


            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;



            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, _width * _height, 0, _width * 2 * (_height - 1) - 2);

            }

            base.Draw(gameTime);
        }

        #region Properties
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        public bool Normals { set { _showNormals = value; } get { return _showNormals; } }

        public BasicEffect Effect
        {
            get { return _effect; }
        }

        #endregion


    }
}
