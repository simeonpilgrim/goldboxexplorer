using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace GoldBoxExplorer.Lib.Plugins.DaxEcl
{
    public class EclFileViewer : IGoldBoxViewer
    {
        public event EventHandler<ChangeFileEventArgs> ChangeSelectedFile;
        private readonly DaxEclFile _file;
        private TabControl tab;
        private int findNext = 0;

        public EclFileViewer(DaxEclFile file)
        {
            _file = file;
        }

        // dynamically create ECL code listing when the tab is clicked on
        private void ECLTabControlUnloadDeselected(Object sender, TabControlEventArgs e)
        {
            if (e.TabPage != null && e.TabPage.Controls.Find("codepanel", false).Length > 0)
            {
                EmptyECLCodePanel(e.TabPage);
            }
        }

        // dynamically create ECL code listing when the tab is clicked on
        private void ECLTabControlLoadSelected(Object sender, TabControlEventArgs e)
        {
            if (e.TabPage != null && e.TabPage.Controls.Find("codepanel", false).Length > 0)
            {
                FillECLCodePanel(e.TabPage);
            }
        }

        public Control GetControl()
        {
            tab = new TabControl { Dock = DockStyle.Fill };

            foreach (var ecl in _file.eclDumps)
            {
                var page = new TabPage(ecl._blockName);
                Panel codePanel = (Panel)ViewerHelper.CreatePanel();
                codePanel.Name = "codepanel";
                codePanel.Tag = ecl;
                //page.AutoScroll = true;
                page.Controls.Add(codePanel);

                // fill the code panel with the decoded ecl code
                int bookMarkIndex = FillECLCodePanel(page);
                // add a search bar and 'select all' button to the top of the ecl listing
                var selectAll = ViewerHelper.CreateButton();
                selectAll.Text = "Copy to clipboard";
                selectAll.MouseClick += selectAllRows;
                selectAll.Dock = DockStyle.Right;

                var findNext = ViewerHelper.CreateButton();
                findNext.Text = "find next";
                findNext.MouseClick += searchEclNext;
                findNext.Dock = DockStyle.Right;

                TextBox headerText = (TextBox)ViewerHelper.CreateTextBox();
                headerText.ReadOnly = false;
                headerText.Text = "Type text to find";
                headerText.TextChanged += searchEcl;
                headerText.KeyDown += searchEclKeyPressed;
                headerText.Dock = DockStyle.Fill;
                var row1 = ViewerHelper.CreateRow();
                page.Controls.Add(row1);
                row1.Controls.Add(headerText);
                row1.Controls.Add(findNext);
                row1.Controls.Add(selectAll);

                tab.TabPages.Add(page);
                if (page.Text == ChangeFileEventArgs.currentDaxId.ToString())
                {
                    tab.SelectedTab = page;
                    ListView lv = (ListView)codePanel.Controls.Find("eclView", false)[0];
                    var lvi = lv.Items[bookMarkIndex];
                    lvi.Selected = true;
                    lv.Select();
                    lv.TopItem = lvi;
                }
            }
            var stringPage = new TabPage("ECL Text");
            stringPage.AutoScroll = true;
            var control = ViewerHelper.CreateTextBoxMultiline();
            control.Text = _file.ToString();
            stringPage.Controls.Add(control);
            tab.TabPages.Add(stringPage);
            tab.Selected += ECLTabControlLoadSelected;
            tab.Deselected += ECLTabControlUnloadDeselected;
            return tab;

        }

        private static void EmptyECLCodePanel(TabPage tabPage)
        {
            Panel codePanel = (Panel)tabPage.Controls.Find("codepanel", false)[0];
            codePanel.Controls.Clear();
        }

        private static int FillECLCodePanel(TabPage page)
        {
            int bookMarkIndex = 0;
            Panel codePanel = (Panel)page.Controls.Find("codepanel", false)[0];
            EclDump.EclDump ecl = (EclDump.EclDump)codePanel.Tag;
            // decode ecl files, and put each line in its own textbox

            var eclView = new ListView();
            var addrColumn = new ColumnHeader();
            var opCodeColumn = new ColumnHeader();
            var opNameColumn = new ColumnHeader();
            var codeColumn = new ColumnHeader();
            var annotationColumn =new ColumnHeader();

            eclView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                addrColumn, opCodeColumn, opNameColumn, codeColumn, annotationColumn });

            eclView.Name = "eclView";
            eclView.Dock = DockStyle.Fill;
            eclView.FullRowSelect = true;
            eclView.MultiSelect = false;
            eclView.TabIndex = 0;
            eclView.UseCompatibleStateImageBehavior = false;
            eclView.View = View.Details;
            eclView.HideSelection = false;
            addrColumn.Text = "Addr";
            addrColumn.Width = 60;
            opCodeColumn.Text = "OpCode";
            opCodeColumn.Width = 60;
            opNameColumn.Text = "OpName";
            opNameColumn.Width = 100;
            codeColumn.Text = "Code";
            codeColumn.Width = 300;
            annotationColumn.Text = "Annotation";
            annotationColumn.Width = 300;

            foreach (var eclAddr in ecl.decodedEcl.Keys)
            {
                string annotation;
                if (ecl.annotations.TryGetValue(eclAddr, out annotation) == false)
                    annotation = "";
                var data = ecl.decodedEcl[eclAddr].Split('|');
                var child = new ListViewItem(data[0]);
                var listViewItem = eclView.Items.Add(child);
                listViewItem.SubItems.Add(data[1]);
                listViewItem.SubItems.Add(data[2]);
                listViewItem.SubItems.Add(data[3]);
                listViewItem.SubItems.Add(annotation);

                if (ChangeFileEventArgs.targetPlace != "" &&
                    annotation.Contains(ChangeFileEventArgs.targetPlace) &&
                    page.Text == ChangeFileEventArgs.currentDaxId.ToString())
                {
                    bookMarkIndex = listViewItem.Index;
                }
            }
            codePanel.Controls.Add(eclView);

            return bookMarkIndex;
        }

        public float Zoom { get; set; }
        public void findInEcl(object sender, int index = 0)
        {
            var findTextBox = (TextBox)sender;
            var text = findTextBox.Text;
            Panel eclListing = (Panel)findTextBox.Parent.Parent.Controls[0];
            string textFound = "";
            // clear all selections
            foreach (var c in eclListing.Controls)
            {
                var co = (System.Windows.Forms.Panel)c;
                var tb = (System.Windows.Forms.TextBox)co.Controls[0];
                tb.HideSelection = true;
            }
            // search through the textboxes to find the text, select it
            for (int i = eclListing.Controls.Count - 1; i > 0; i--)
            {
                System.Windows.Forms.Panel rowControl = (System.Windows.Forms.Panel)eclListing.Controls[i];
                System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)rowControl.Controls[0];
                System.Windows.Forms.TextBox atb = (System.Windows.Forms.TextBox)rowControl.Controls[1];
                int textStart = tb.Text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);

                if (textStart > -1)
                {
                    if (index <= 1)
                    {
                        scrollIntoViewAndHighlight(text, eclListing, rowControl, tb, textStart);
                        return;
                    } else
                        index--;
                } else
                {
                    // can't find the text in the ecl code textbox, so try the annotations textbox next to it
                    textStart = atb.Text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase);
                    if (textStart > -1)
                    {
                        if (index <= 1)
                        {
                            scrollIntoViewAndHighlight(text, eclListing, rowControl, atb, textStart);
                            return;
                        } else
                            index--;
                    }

                }
            }

            // nothing found! reset the find next counter
            findNext = 0;

        }

        private static void scrollIntoViewAndHighlight(string text, Panel eclListing, System.Windows.Forms.Panel co, System.Windows.Forms.TextBox tb, int textStart)
        {
            eclListing.ScrollControlIntoView(co);
            tb.Select(textStart, text.Length);
            tb.HideSelection = false;
            return;
        }

        public void searchEcl(object sender, EventArgs e)
        {
            findInEcl(sender);
        }
        public void searchEclNext(object sender, MouseEventArgs e)
        {
            var b = (System.Windows.Forms.ButtonBase)sender;
            findNext++;
            findInEcl(b.Parent.Controls[0], findNext);
        }
        void searchEclKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var b = (System.Windows.Forms.TextBoxBase)sender;
                findNext++;
                findInEcl(b.Parent.Controls[0], findNext);
            }
        }
        public void selectAllRows(object sender, MouseEventArgs e)
        {
            // select all the ecl code in the panel and send it to the clipboard
            var b = (System.Windows.Forms.ButtonBase)sender;
            var eclListing = b.Parent.Parent.Controls[0];
            ListView eclRows = (ListView)eclListing.Controls[0];
            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem lvi in eclRows.Items)
            {
                sb.AppendLine($"{lvi.SubItems[0].Text} {lvi.SubItems[1].Text} {lvi.SubItems[2].Text} {lvi.SubItems[3].Text}");
            }
            Clipboard.SetText(sb.ToString());
        }

        public int ContainerWidth { get; set; }
    }
}