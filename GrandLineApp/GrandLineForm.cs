using GrandLineLib.Data;
using GrandLineLib;
using GrandLineLib.Exel;

namespace AppWindowsForm
{
    public partial class GrandLineForm : Form
    {
        private GrandLine? _grandLine;

        public GrandLineForm()
        {
            InitializeComponent();
            LoadData();
            trackBarSpeed.Maximum = 20000;
            trackBarSpeed.Minimum = 100;
            trackBarSpeed.Value = 10000;
        }

        private void LoadData()
        {
            _grandLine = new GrandLine("ca53919db52e201246b7d2a7f5b73753", false);

            listBoxBranches.Items.Clear();
            listBoxBranches.Items.AddRange(_grandLine.Branches!.ToArray());

            listBoxAgreements.Items.Clear();
            listBoxAgreements.Items.AddRange(_grandLine.Agreements!.ToArray());
        }

        private async void buttonCreateTable_Click(object sender, EventArgs e)
        {
            if (listBoxAgreements.SelectedItem == null || listBoxBranches.SelectedItem == null)
            {
                MessageBox.Show("�������� ����� �������� � �������");
                return;
            }

            var agreement = listBoxAgreements.SelectedItem as Agreement;
            var branch = listBoxBranches.SelectedItem as Branche;
            var numberOfObjects = trackBarSpeed.Value;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "����� Exel (.xlsx)|*.xlsx";
            sf.ShowDialog();
            var path = sf.FileName;

            if (path == "")
                return;

            MessageBox.Show("�������� �������� �����\n������� '��' � �������� ��������� ���������");

            labelInfoLoad.Text = "��� �������� �����";
            timerAnimationLoading.Enabled = true;
            buttonCreateTable.Enabled = false;

            await Task.Run(() => CreateTable(agreement, branch, path, numberOfObjects)).WaitAsync(new TimeSpan(2, 30, 0), TimeProvider.System);
            labelInfoLoad.Text = "";
            buttonCreateTable.Enabled = true;
            timerAnimationLoading.Enabled = false;
        }

        private async Task CreateTable(Agreement? agreement, Branche? branch, string path, int numberOfObjects)
        {
            try
            {
                await Task.Run(() => _grandLine!.FullLoadingUpdatingOfTables([agreement!.id_1c], [branch!.id_1c], numberOfObjects)).WaitAsync(new TimeSpan(2, 30, 0), TimeProvider.System);

                GrandLineTableExel grandLineTable = new GrandLineTableExel(_grandLine!);
                grandLineTable.CreateTable(path);

                MessageBox.Show("���� ������� ������");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�������� ������ �������� �����\n\n{ex}");
            }
        }

        private void listBoxBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxBranches.SelectedItem != null)
            {
                Branche br = (Branche)listBoxBranches.SelectedItem;
                richTextBoxInfo.Text = $"��� 1c = {br.code_1c}\n\n" +
                                       $"�������� = {br.name}\n\n" +
                                       $"����� = {br.address}\n\n" +
                                       $"����������� � ����� �������� = {br.description}";
            }
        }

        private void listBoxAgreements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAgreements.SelectedItem != null)
            {
                Agreement ag = (Agreement)listBoxAgreements.SelectedItem;
                richTextBoxInfo.Text = $"��� 1c = {ag.code_1c}\n\n" +
                                       $"�������� = {ag.name}\n\n" +
                                       $"������ ���. ���������� = {string.Join(" ", ag.additional_agreements!)}\n";

            }
        }

        private void timerAnimationLoading_Tick(object sender, EventArgs e)
        {
            if (labelInfoLoad.Text.Where(i => i == '.').Count() < 3)
            {
                labelInfoLoad.Text += ".";
            }
            else
            {
                labelInfoLoad.Text = labelInfoLoad.Text.Trim('.');
            }
        }

        private void panel7_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
