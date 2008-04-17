using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SourceGrid3;
using SourceGrid3.Cells.Views;
using Microsoft.AnalysisServices.AdomdClient;
using System.Reflection;

namespace ADOMD_Csharp_example
{
    public partial class frmSchemaInfo : Form
    {
        private AdomdConnection moConn;

        public frmSchemaInfo(AdomdConnection conn)
        {
            InitializeComponent();
            moConn = conn;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddValues("Actions", AdomdSchemaGuid.Actions);
            AddValues("Catalogs", AdomdSchemaGuid.Catalogs);
            AddValues("Columns", AdomdSchemaGuid.Columns);
            AddValues("Connections", AdomdSchemaGuid.Connections);
            AddValues("Cubes", AdomdSchemaGuid.Cubes);
            AddValues("DataSources", AdomdSchemaGuid.DataSources);
            AddValues("DBConnections", AdomdSchemaGuid.DBConnections);
            AddValues("Dimensions", AdomdSchemaGuid.Dimensions);
            AddValues("DimensionStat", AdomdSchemaGuid.DimensionStat);
            AddValues("Enumerators", AdomdSchemaGuid.Enumerators);
            AddValues("Functions", AdomdSchemaGuid.Functions);
            AddValues("Hierarchies", AdomdSchemaGuid.Hierarchies);
            AddValues("InputDataSources", AdomdSchemaGuid.InputDataSources);
            AddValues("Instances", AdomdSchemaGuid.Instances);
            AddValues("Jobs", AdomdSchemaGuid.Jobs);
            AddValues("Keywords", AdomdSchemaGuid.Keywords);
            AddValues("Kpis", AdomdSchemaGuid.Kpis);
            AddValues("Levels", AdomdSchemaGuid.Levels);
            AddValues("Literals", AdomdSchemaGuid.Literals);
            AddValues("Locations", AdomdSchemaGuid.Locations);
            AddValues("Locks", AdomdSchemaGuid.Locks);
            AddValues("MasterKey", AdomdSchemaGuid.MasterKey);
            AddValues("MeasureGroupDimensions", AdomdSchemaGuid.MeasureGroupDimensions);
            AddValues("MeasureGroups", AdomdSchemaGuid.MeasureGroups);
            AddValues("Measures", AdomdSchemaGuid.Measures);
            AddValues("MemberProperties", AdomdSchemaGuid.MemberProperties);
            AddValues("Members", AdomdSchemaGuid.Members);
            AddValues("MemoryGrant", AdomdSchemaGuid.MemoryGrant);
            AddValues("MemoryUsage", AdomdSchemaGuid.MemoryUsage);
            AddValues("MiningColumns", AdomdSchemaGuid.MiningColumns);
            AddValues("MiningFunctions", AdomdSchemaGuid.MiningFunctions);
            AddValues("MiningModelContent", AdomdSchemaGuid.MiningModelContent);
            AddValues("MiningModelContentPmml", AdomdSchemaGuid.MiningModelContentPmml);
            AddValues("MiningModels", AdomdSchemaGuid.MiningModels);
            AddValues("MiningModelXml", AdomdSchemaGuid.MiningModelXml);
            AddValues("MiningServiceParameters", AdomdSchemaGuid.MiningServiceParameters);
            AddValues("MiningServices", AdomdSchemaGuid.MiningServices);
            AddValues("MiningStructureColumns", AdomdSchemaGuid.MiningStructureColumns);
            AddValues("MiningStructures", AdomdSchemaGuid.MiningStructures);
            AddValues("PartitionDimensionStat", AdomdSchemaGuid.PartitionDimensionStat);
            AddValues("PartitionStat", AdomdSchemaGuid.PartitionStat);
            AddValues("PerformanceCounters", AdomdSchemaGuid.PerformanceCounters);
            AddValues("ProviderTypes", AdomdSchemaGuid.ProviderTypes);
            AddValues("SchemaRowsets", AdomdSchemaGuid.SchemaRowsets);
            AddValues("Sessions", AdomdSchemaGuid.Sessions);
            AddValues("Sets", AdomdSchemaGuid.Sets);
            AddValues("Tables", AdomdSchemaGuid.Tables);
            AddValues("TablesInfo", AdomdSchemaGuid.TablesInfo);
            AddValues("TraceColumns", AdomdSchemaGuid.TraceColumns);
            AddValues("TraceDefinitionProviderInfo", AdomdSchemaGuid.TraceDefinitionProviderInfo);
            AddValues("TraceEventCategories", AdomdSchemaGuid.TraceEventCategories);
            AddValues("Traces", AdomdSchemaGuid.Traces);
            AddValues("Transactions", AdomdSchemaGuid.Transactions);
            AddValues("XmlaProperties", AdomdSchemaGuid.XmlaProperties);
            AddValues("XmlMetadata", AdomdSchemaGuid.XmlMetadata);
        }

        private class SchemaValues
        {
            public SchemaValues(string name, Guid value)
            {
                msName = name;
                mgValue = value;
            }

            string msName = "";
            Guid mgValue; // = null;

            public string Name
            {
                get { return msName; }
            }
            public Guid Value
            {
                get { return mgValue; }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private void AddValues(string name, Guid value)
        {
            comboBox1.Items.Add(new SchemaValues(name, value)); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SchemaValues schema = (SchemaValues)comboBox1.SelectedItem;
            LoadSchema(schema);
        }

        private void LoadSchema(SchemaValues schema)
        {
            try
            {
                AdomdRestrictionCollection oList = new AdomdRestrictionCollection();

                object[] oValues = new object[0];
                //if (schema.Name == "Dimensions")
                //{
                //    oValues = new object[2];
                //    oValues[0] = "CATALOG_NAME";
                //    oValues[1] = "Adventure Works";
                //}
                DataSet ds = moConn.GetSchemaDataSet(schema.Value, oValues);

                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = ds.Tables[0].TableName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to display values for that item, it probably requires restrictions\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            SchemaValues schema = (SchemaValues)comboBox1.SelectedItem;
            LoadSchema(schema);
        }
    }
}