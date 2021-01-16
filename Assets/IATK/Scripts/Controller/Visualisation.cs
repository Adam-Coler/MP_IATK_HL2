using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;


namespace IATK
{
   
    /// <summary>
    /// Visualisation class to act as a view controller - reads the model to create the view
    /// </summary>
    [ExecuteInEditMode]
    public class Visualisation : MonoBehaviour
    {
       
        //// DATA
        [Tooltip("The source for the data")]
        public DataSource dataSource;

        [Tooltip("The type of geometry to display")]
        public AbstractVisualisation.GeometryType geometry;

        [Tooltip("The colour of the geometry")]
        public Color colour = Color.white;

        [Tooltip("The size of the geometry")]
        [Range(0.0f, 1.0f)]
        public float size = 0.3f;

        [Tooltip("The minimum size of the geometry")]
        [Range(0.0f, 1.0f)]
        public float minSize = 0.01f;

        [Tooltip("The maximum size of the geometry")]
        [Range(0.0f, 1.0f)]
        public float maxSize = 1.0f;

        [Tooltip("The X dimension")]
        public DimensionFilter xDimension = new DimensionFilter { Attribute = "Undefined" };

        [Tooltip("The Y dimension")]
        public DimensionFilter yDimension = new DimensionFilter { Attribute = "Undefined" };

        [Tooltip("The Z dimension")]
        public DimensionFilter zDimension = new DimensionFilter { Attribute = "Undefined" };

        public AttributeFilter[] attributeFilters;

        [Tooltip("The x dimensions represented in a scatterplot matrix")]
        [SerializeField]
        public DimensionFilter[] xScatterplotMatrixDimensions;

        [Tooltip("The y dimensions represented in a scatterplot matrix")]
        [SerializeField]
        public DimensionFilter[] yScatterplotMatrixDimensions;

        [Tooltip("The z dimensions represented in a scatterplot matrix")]
        [SerializeField]
        public DimensionFilter[] zScatterplotMatrixDimensions;

        [Tooltip("The dimensions of the Parallel Coordinates")]
        [SerializeField]
        public DimensionFilter[] parallelCoordinatesDimensions;

        [Tooltip("The dimension to map the colour to")]
        public string colourDimension = "Undefined";

        [Tooltip("The colour gradient used to map to the colour data dimension")]
        public Gradient dimensionColour;

        [Tooltip("The blending mode source")]
        public string blendingModeSource = UnityEngine.Rendering.BlendMode.SrcAlpha.ToString();

        [Tooltip("The blending mode destination")]
        public string blendingModeDestination = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha.ToString();

        [Tooltip("The dimension to map the size to")]
        public string sizeDimension = "Undefined";

        [Tooltip("The dimension that links data points for trajectories")]
        public string linkingDimension = "Undefined";

        [Tooltip("The dimension that links origin points to destination")]
        public string originDimension = "Undefined";

        [Tooltip("The dimension that links origin points to destination")]
        public string destinationDimension = "Undefined";

        [Tooltip("The graph dimension that links data points for networks")]
        public string graphDimension = "Undefined";

        [Tooltip("The number of loaded data points")]
        public string dataPoints = "";

        [Tooltip("The number of dimensions")]
        public string dataDimensions = "";

        [Tooltip("The size of the font for the axes labels")]
        public int fontAxesSize = 500;

        [Tooltip("The type of the visualisation you want to display")]
        public AbstractVisualisation.VisualisationTypes visualisationType;// = AbstractViualisation.VisualisationTypes.SIMPLE_VISUALISATION;

        [Tooltip("The color palette for discrete variables mapping")]
        public Color[] coloursPalette;
        
        public float width = 1.0f;
        public float height = 1.0f;
        public float depth = 1.0f;

        public string colorPaletteDimension = "Undefined";

        [HideInInspector]
        public AbstractVisualisation theVisualizationObject;// = null;

        // Unique ID for visualisation creation
        public string uid = null;

        public delegate void UpdateViewAction(AbstractVisualisation.PropertyType propertyType);
        public static event UpdateViewAction OnUpdateViewAction;

        //Private
        // Key
        GameObject key;

        int MAX_INIT_SCATTERPLOTMATRIX = 5;


        // PUBLIC
        public void CreateVisualisation(AbstractVisualisation.VisualisationTypes visualizationType)
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "CreateVisualisation - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            //destroy the previous visualisations
            AbstractVisualisation[] previousVisualizations = GetComponentsInChildren<AbstractVisualisation>();

            foreach (var item in previousVisualizations)
            {
                item.destroyView();
                DestroyImmediate(item);
            }

            //destroy the previous axes
            Axis[] previousAxes = GetComponentsInChildren<Axis>();

            foreach (var item in previousAxes)
            {
                DestroyImmediate(item.gameObject);
            }

            //destroy previous key
            if(key!=null)
            DestroyImmediate(key.gameObject);

            visualisationType = visualizationType;

            switch (visualisationType)
            {
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                    theVisualizationObject = gameObject.AddComponent<ScatterplotVisualisation>();// new Simple2D3DVisualisation();                    
                    theVisualizationObject.visualisationReference = this;

                    //Debug.LogFormat(Photon_IATK.GlobalVariables.cRegister + "{0}." + Photon_IATK.GlobalVariables.endColor + " {1}: {2} -> {3} -> {4}", "theVisObject made", Time.realtimeSinceStartup, this.gameObject.name, this.GetType(), System.Reflection.MethodBase.GetCurrentMethod());

                    theVisualizationObject.CreateVisualisation();

                    break;
                case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:
        
                    int dimensionCount = dataSource.DimensionCount;
                    if (dimensionCount > MAX_INIT_SCATTERPLOTMATRIX) dimensionCount = MAX_INIT_SCATTERPLOTMATRIX;

                    xScatterplotMatrixDimensions = new DimensionFilter[dimensionCount];
                    yScatterplotMatrixDimensions = new DimensionFilter[dimensionCount];

                    for (int i = 0; i < dimensionCount; i++)
                    {
                        xScatterplotMatrixDimensions[i] = new DimensionFilter { Attribute = dataSource[i].Identifier };
                        yScatterplotMatrixDimensions[i] = new DimensionFilter { Attribute = dataSource[i].Identifier };
                    }

                    theVisualizationObject = gameObject.AddComponent<ScatterplotMatrixVisualisation>();// new Simple2D3DVisualisation();                    
                    theVisualizationObject.visualisationReference = this;

                    theVisualizationObject.CreateVisualisation();
                    break;
                case AbstractVisualisation.VisualisationTypes.PARALLEL_COORDINATES:
                    parallelCoordinatesDimensions = new DimensionFilter[dataSource.DimensionCount];

                    for (int i = 0; i < dataSource.DimensionCount; i++)
                    {
                        parallelCoordinatesDimensions[i] = new DimensionFilter { Attribute = dataSource[i].Identifier };
                    }
                    theVisualizationObject = gameObject.AddComponent<ParallelCoordinatesVisualisation>();// new ParrallelCoordinates();
                    
                    theVisualizationObject.visualisationReference = this;
                    theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.DimensionChange);

                    theVisualizationObject.CreateVisualisation();


                    break;
                case AbstractVisualisation.VisualisationTypes.GRAPH_LAYOUT:
                    break;
                default:
                    break;
            }

            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.None);

            RuntimeEditorLoadAndSaveConfiguration();

            key = (GameObject)Instantiate(Resources.Load("Key"));
            key.transform.parent = transform;
            key.transform.localPosition = new Vector3(0.15f, 1.165f, 0f);
        }

        public void updateView(AbstractVisualisation.PropertyType propertyType)
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "updateView - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType() + " : " + this.gameObject.name, propertyType, Time.realtimeSinceStartup);

            theVisualizationObject.CreateVisualisation();// UpdateVisualisation(propertyType);
        }
        
        /// <summary>
        /// Gets the axies.
        /// </summary>
        /// <returns>The axies.</returns>
        /// <param name="axies">Axies.</param>
        private string getAxis(Dictionary<CreationConfiguration.Axis, string> axies, CreationConfiguration.Axis axis)
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "getAxis - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            string axes = null;
            string retVal = "";
            if (axies.TryGetValue(axis, out axes))
                retVal = axes;

            return retVal;
        }
        
        public void updateViewProperties(AbstractVisualisation.PropertyType propertyType)
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "updateViewProperties - {0}, {1}, Property Type: {2}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup, propertyType.ToString());

            if (theVisualizationObject == null) CreateVisualisation(visualisationType);
            theVisualizationObject.UpdateVisualisation(propertyType);

            if(OnUpdateViewAction!=null)
            OnUpdateViewAction(propertyType);

            if (key != null)
            {
                updateKey();
            }
        }

        public void updateProperties()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "updateProperties - {0}, {1}, theVisualizationObject exists: {2}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup, theVisualizationObject != null);

            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "AttributeFiltering");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.AttributeFiltering);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "BlendDestinationMode");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.BlendDestinationMode);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "BlendSourceMode");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.BlendSourceMode);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "Colour");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Colour);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "DimensionChange");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.DimensionChange);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "DimensionFiltering");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.DimensionFiltering);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "GeometryType");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.GeometryType);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "LinkingDimension");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.LinkingDimension);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "None");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.None);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "Scaling");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Scaling);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "Size");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Size);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "SizeValues");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.SizeValues);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "VisualisationType");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.VisualisationType);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "X");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.X);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "Y");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Y);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "Z");
            theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Z);
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "{0}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), "END");

        }

        /// <summary>
        /// creates the line indices for the data set
        /// </summary>
        /// <param name="LinkingIndices"></param>
        /// <returns></returns>
        public int[] setLinkingIndices(float[] LinkingIndices)
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "setLinkingIndices - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            List<int> indices = new List<int>();

            {
                for (int i = 0; i < LinkingIndices.Length; i++)
                {
                    if (LinkingIndices[i] == LinkingIndices[i + 1])
                    {
                        indices.Add(i);
                        indices.Add(i + 1);
                    }
                }
            }

            return indices.ToArray();
        }

        private void updateKey()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "updateKey - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            key.GetComponent<Key>().UpdateProperties(AbstractVisualisation.PropertyType.None, this);

            if (yDimension.Attribute != "Undefined")
            {
                key.transform.localPosition = new Vector3(0.2f, height + 0.25f, 0f);
            }
            else
            {
                key.transform.localPosition = new Vector3(0.2f, 0.2f, 0f);
            }
        }


        void OnEnable()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "OnEnable - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            if (uid == null)
            {
                uid = Guid.NewGuid().ToString().Substring(0, 8);
            }

            if (theVisualizationObject != null)
                RuntimeEditorLoadAndSaveConfiguration();

        }

        void RuntimeEditorLoadAndSaveConfiguration()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "RuntimeEditorLoadAndSaveConfiguration - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            // get the pre existing views in the hierarchy
            View[] views = GetComponentsInChildren<View>();

            // clear the reference list of views
            theVisualizationObject.viewList.Clear();

            // create the new view reference list
            foreach (var view in views)
            {
                view.BigMesh = view.GetComponentInChildren<BigMesh>();
                view.onViewChangeEvent += updateView;   // Receive notifications when the view configuration changes
                theVisualizationObject.viewList.Add(view);
            }
            
            // bind the axes objects by the name of the property in the children hierarchy
            foreach (Transform child in transform)
            {
                AbstractVisualisation.PropertyType pt;
                try
                {
                    pt = (AbstractVisualisation.PropertyType)System.Enum.Parse(typeof(AbstractVisualisation.PropertyType), child.gameObject.name);
                    switch (pt)
                    {
                        case AbstractVisualisation.PropertyType.X:
                            theVisualizationObject.X_AXIS = child.gameObject;
                            break;
                        case AbstractVisualisation.PropertyType.Y:
                            theVisualizationObject.Y_AXIS = child.gameObject;
                            break;
                        case AbstractVisualisation.PropertyType.Z:
                            theVisualizationObject.Z_AXIS = child.gameObject;
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    //handle parsing property errors here
                }
            }

            // load serialized view configuration from disk
            if (File.Exists(ConfigurationFileName()))
            {
                if (theVisualizationObject.creationConfiguration == null) theVisualizationObject.creationConfiguration = new CreationConfiguration();
                if (!dataSource.IsLoaded) dataSource.load();

                theVisualizationObject.creationConfiguration.Deserialize(ConfigurationFileName());
                theVisualizationObject.creationConfiguration.disableWriting = true;

                visualisationType = theVisualizationObject.creationConfiguration.VisualisationType;

                switch (visualisationType)
                {
                    case AbstractVisualisation.VisualisationTypes.SCATTERPLOT:
                        if (theVisualizationObject.creationConfiguration.Axies.ContainsKey(CreationConfiguration.Axis.X)) xDimension.Attribute = theVisualizationObject.creationConfiguration.Axies[CreationConfiguration.Axis.X];
                        if (theVisualizationObject.creationConfiguration.Axies.ContainsKey(CreationConfiguration.Axis.Y)) yDimension.Attribute = theVisualizationObject.creationConfiguration.Axies[CreationConfiguration.Axis.Y];
                        if (theVisualizationObject.creationConfiguration.Axies.ContainsKey(CreationConfiguration.Axis.Z)) zDimension.Attribute = theVisualizationObject.creationConfiguration.Axies[CreationConfiguration.Axis.Z];

                        linkingDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.LinkingDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.LinkingDimension;
                        geometry = theVisualizationObject.creationConfiguration.Geometry;
                        minSize = theVisualizationObject.creationConfiguration.MinSize;
                        maxSize = theVisualizationObject.creationConfiguration.MaxSize;

                        theVisualizationObject.CreateVisualisation();

                        updateViewProperties(AbstractVisualisation.PropertyType.SizeValues);
                        updateViewProperties(AbstractVisualisation.PropertyType.X);
                        updateViewProperties(AbstractVisualisation.PropertyType.Y);
                        updateViewProperties(AbstractVisualisation.PropertyType.Z);
                        updateViewProperties(AbstractVisualisation.PropertyType.GeometryType);
                        updateViewProperties(AbstractVisualisation.PropertyType.LinkingDimension);

                        colourDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.ColourDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.ColourDimension;
                        sizeDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.SizeDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.SizeDimension;
                        dimensionColour = theVisualizationObject.creationConfiguration.colourKeys;
                        colour = theVisualizationObject.creationConfiguration.colour;

                        updateViewProperties(AbstractVisualisation.PropertyType.Size);
                        updateViewProperties(AbstractVisualisation.PropertyType.Colour);
                        
                        width = theVisualizationObject.creationConfiguration.VisualisationWidth;
                        height = theVisualizationObject.creationConfiguration.VisualisationHeight;
                        depth = theVisualizationObject.creationConfiguration.VisualisationDepth;
                        
                        updateViewProperties(AbstractVisualisation.PropertyType.Scaling);

                        break;
                        
                    case AbstractVisualisation.VisualisationTypes.SCATTERPLOT_MATRIX:

                        linkingDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.LinkingDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.LinkingDimension;
                        geometry = theVisualizationObject.creationConfiguration.Geometry;
                        minSize = theVisualizationObject.creationConfiguration.MinSize;
                        maxSize = theVisualizationObject.creationConfiguration.MaxSize;

                        theVisualizationObject.CreateVisualisation();

                        updateViewProperties(AbstractVisualisation.PropertyType.SizeValues);
                        updateViewProperties(AbstractVisualisation.PropertyType.X);
                        updateViewProperties(AbstractVisualisation.PropertyType.Y);
                        updateViewProperties(AbstractVisualisation.PropertyType.Z);
                        updateViewProperties(AbstractVisualisation.PropertyType.GeometryType);
                        updateViewProperties(AbstractVisualisation.PropertyType.LinkingDimension);
                        theVisualizationObject.creationConfiguration.Deserialize(ConfigurationFileName());
                        colourDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.ColourDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.ColourDimension;
                        sizeDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.SizeDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.SizeDimension;
                        dimensionColour = theVisualizationObject.creationConfiguration.colourKeys;
                        colour = theVisualizationObject.creationConfiguration.colour;

                        updateViewProperties(AbstractVisualisation.PropertyType.Size);
                        updateViewProperties(AbstractVisualisation.PropertyType.Colour);

                        width = theVisualizationObject.creationConfiguration.VisualisationWidth;
                        height = theVisualizationObject.creationConfiguration.VisualisationHeight;
                        depth = theVisualizationObject.creationConfiguration.VisualisationDepth;
                        
                        updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
                        
                        break;
                        
                    case AbstractVisualisation.VisualisationTypes.PARALLEL_COORDINATES:
                        parallelCoordinatesDimensions = theVisualizationObject.creationConfiguration.parallelCoordinatesDimensions;
                        updateViewProperties(AbstractVisualisation.PropertyType.DimensionChange);
                        size = theVisualizationObject.creationConfiguration.Size;
                        minSize = theVisualizationObject.creationConfiguration.MinSize;
                        maxSize = theVisualizationObject.creationConfiguration.MaxSize;
                        updateViewProperties(AbstractVisualisation.PropertyType.SizeValues);
                        // TEMP_FIX:
                        // Issue: for some reason after updateViewProperties(PropertyType.DimensionChange) call
                        // the creationConfiguration object is overwritten and some properties take old values
                        // the temp fix is to deserialize again to read the correct values again. I suspect this
                        // is because the script is using an old pre-runtime reference.
                        theVisualizationObject.creationConfiguration.Deserialize(ConfigurationFileName());
                        colourDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.ColourDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.ColourDimension;
                        sizeDimension = string.IsNullOrEmpty(theVisualizationObject.creationConfiguration.SizeDimension) ? "Undefined" : theVisualizationObject.creationConfiguration.SizeDimension;
                        dimensionColour = theVisualizationObject.creationConfiguration.colourKeys;
                        colour = theVisualizationObject.creationConfiguration.colour;

                        updateViewProperties(AbstractVisualisation.PropertyType.Size);
                        updateViewProperties(AbstractVisualisation.PropertyType.Colour);

                        width = theVisualizationObject.creationConfiguration.VisualisationWidth;
                        height = theVisualizationObject.creationConfiguration.VisualisationHeight;
                        depth = theVisualizationObject.creationConfiguration.VisualisationDepth;
                        
                        updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
                        
                        break;
                        
                    case AbstractVisualisation.VisualisationTypes.GRAPH_LAYOUT:
                        break;
                        
                    default:
                        break;
                }


                theVisualizationObject.creationConfiguration.disableWriting = false;
            }
        }

        private string ConfigurationFileName()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "ConfigurationFileName - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            string PathName = Application.streamingAssetsPath + Path.DirectorySeparatorChar + theVisualizationObject.serializedObjectPath;
            return PathName + Path.DirectorySeparatorChar + uid + ".json";
        }

        //<summary>
        //Destroy immediately all the views
        //</summary>
        void destroyView()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "destroyView - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            string backupname = name;
            List<GameObject> children = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<View>() != null)
                    children.Add(transform.GetChild(i).gameObject);
            }

            foreach (var item in children)
            {
                DestroyImmediate(item);
            }
            name = backupname;

            if(key!=null) DestroyImmediate(key);

        }

        void OnApplicationQuit()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "OnApplicationQuit - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            if (theVisualizationObject.creationConfiguration != null)
                theVisualizationObject.SerializeViewConfiguration(theVisualizationObject.creationConfiguration);
        }

        void OnDestroy()
        {
            //Debug.LogFormat(Photon_IATK.GlobalVariables.yellow + "OnDestroy - {0}, {1}" + Photon_IATK.GlobalVariables.endColor + " : " + this.GetType(), this.gameObject.name, Time.realtimeSinceStartup);

            destroyView();

        }

        

        
    }

}   // Namespace