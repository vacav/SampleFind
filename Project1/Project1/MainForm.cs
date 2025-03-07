using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;

public partial class MainForm : Form
{
    private List<string> allLines = new List<string>();
    private List<FilterCondition> filterConditions = new List<FilterCondition>();

    public MainForm()
    {
        InitializeComponent();
        SetupUI();
    }

    private void SetupUI()
    {
        // Main form settings
        this.Size = new System.Drawing.Size(800, 600);
        this.Text = "Log File Viewer";

        // Create menu strip
        MenuStrip menuStrip = new MenuStrip();
        ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
        ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Open", null, OpenFile);
        fileMenu.DropDownItems.Add(openMenuItem);
        menuStrip.Items.Add(fileMenu);
        this.Controls.Add(menuStrip);

        // Create filter panel
        Panel filterPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100
        };

        Button addFilterButton = new Button
        {
            Text = "Add Filter",
            Location = new System.Drawing.Point(10, 10)
        };
        addFilterButton.Click += AddFilterButton_Click;
        filterPanel.Controls.Add(addFilterButton);

        Button applyFiltersButton = new Button
        {
            Text = "Apply Filters",
            Location = new System.Drawing.Point(100, 10)
        };
        applyFiltersButton.Click += ApplyFiltersButton_Click;
        filterPanel.Controls.Add(applyFiltersButton);

        this.Controls.Add(filterPanel);

        // Create text display
        TextBox textDisplay = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            Name = "textDisplay"
        };
        this.Controls.Add(textDisplay);
    }

    private void OpenFile()
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                allLines = File.ReadAllLines(openFileDialog.FileName).ToList();
                UpdateDisplay(allLines);
            }
        }
    }

    private void AddFilterButton_Click(object sender, EventArgs e)
    {
        FilterDialog filterDialog = new FilterDialog();
        if (filterDialog.ShowDialog() == DialogResult.OK)
        {
            filterConditions.Add(filterDialog.FilterCondition);
        }
    }

    private void ApplyFiltersButton_Click(object sender, EventArgs e)
    {
        if (filterConditions.Count == 0)
        {
            UpdateDisplay(allLines);
            return;
        }

        var filteredLines = allLines.Where(line =>
            filterConditions.Any(condition => condition.Matches(line))).ToList();

        UpdateDisplay(filteredLines);
    }

    private void UpdateDisplay(List<string> lines)
    {
        TextBox textDisplay = (TextBox)Controls.Find("textDisplay", true)[0];
        textDisplay.Text = string.Join(Environment.NewLine, lines);
    }
} 