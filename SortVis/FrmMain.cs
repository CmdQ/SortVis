using SortLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortVis
{
    /// <summary>
    /// Main form of application.
    /// </summary>
    public partial class FrmMain : Form
    {
        #region Fields

        private CompositionContainer _container;
        private List<ISorter> _sorters;
        private List<IGenerator> _generators;
        private CancellationTokenSource _cts;
        private int[] _numbers;
        private Size _drawSize;

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

            var pi = DgvSorters.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(DgvSorters, true);

            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof(Program).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(".", "*.dll"));

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);

            _sorters = new List<ISorter>();
            foreach (var sorter in MefSorters)
            {
                sorter.Value.Name = sorter.Metadata.Name;
                sorter.Value.BigO = GetLambda(sorter.Value.GetType().Assembly);
                if (SortingCorrect(sorter.Value))
                {
                    _sorters.Add(sorter.Value);
                }
                else
                {
                    Trace.TraceInformation("Not adding {0}, because it does not work correctly.", sorter.Value.Name);
                }
            }

            _generators = new List<IGenerator>();
            foreach (var gen in MefGenerators)
            {
                gen.Value.Name = gen.Metadata.Name;
                var rg = gen.Value as RandomGenerator;
                if (rg != null)
                {
                    rg.Seed = Properties.Settings.Default.RandomSeed == 0
                        ? (new Random()).Next()
                        : Properties.Settings.Default.RandomSeed;
                }
                _generators.Add(gen.Value);
            }

            CmbGenerator.Items.AddRange((from gen in _generators orderby gen.Name select gen.Name).ToArray());
            CmbGenerator.SelectedIndex = CmbGenerator.Items.IndexOf(_generators.Single(g => g is RandomGenerator).Name);

            DgvSorters.DataSource = _sorters;
            for (int i = 0; i < DgvSorters.RowCount; ++i)
            {
                // TODO
                DgvSorters.Rows[i].Cells["Run"].Value = _sorters[i].BigO != BigO.Squared;
            }
        }

        /// <summary>
        /// Used by MEF for sorter plugin loading.
        /// </summary>
        [ImportMany(typeof(ISorter))]
        public IEnumerable<Lazy<ISorter, ISorterMetadata>> MefSorters { get; set; }

        /// <summary>
        /// Used by MEF for generator plugin loading.
        /// </summary>
        [ImportMany(typeof(IGenerator))]
        public IEnumerable<Lazy<IGenerator, IGeneratorMetadata>> MefGenerators { get; set; }

        private IGenerator Generator
        {
            get
            {
                return _generators.Single(g => g.Name == (string)CmbGenerator.SelectedItem);
            }
        }

        private int SortLength
        {
            get
            {
                return (int)NumCount.Value;
            }
        }

        #region Event handlers

        private void BtnAll_Click(object sender, EventArgs e)
        {
            EnableUI(false);

            _cts = new CancellationTokenSource();
            var barrier = new Barrier(_sorters.Count, DrawArrays);
            var tasks = new List<Task>(_sorters.Count);

            var ui = SynchronizationContext.Current;
            for (int i = 0; i < _sorters.Count; ++i)
            {
                var run = true;//TODO
                if (run)
                {
                    var sorter = _sorters[i];
                    sorter.Abort = _cts.Token;
                    sorter.SteppedExecution = barrier;
                    sorter.Numbers = _numbers;
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        sorter.Sort();
                        sorter.SteppedExecution = null;
                    }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current));
                }
            }

            Task.WhenAll(tasks).ContinueWith(_ =>
            {
                ui.Post(delegate
                {
                    EnableUI(true);
                    DrawArrays(null);
                    DgvSorters.Refresh();
                }, null);
                barrier.Dispose();
                _cts.Dispose();
                _cts = null;
            });
        }

        private void NumCount_ValueChanged(object sender, EventArgs e)
        {
            InputArrayChanged();
        }

        private void CmbGenerator_SelectedValueChanged(object sender, EventArgs e)
        {
            InputArrayChanged();
        }

        private void DgvSorters_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column is DataGridViewImageColumn)
            {
                UpdateBitmapSize();
            }
        }

        private void DgvSorters_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
        {
            UpdateBitmapSize();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Redraw();
        }

        private void BtnAbort_Click(object sender, EventArgs e)
        {
            if (_cts != null && _cts.Token.CanBeCanceled)
            {
                _cts.Cancel();
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested && _cts.Token.CanBeCanceled)
            {
                _cts.Cancel();
            }
        }

        #endregion

        #region Private helpers

        private void EnableUI(bool onOff)
        {
            var toDisable = new List<Control>()
            {
                BtnAll,
                NumCount,
                CmbGenerator,
            };
            var toEnable = new List<Control>()
            {
                BtnAbort,
            };

            foreach (var disable in toDisable)
            {
                disable.Enabled = onOff;
            }
            foreach (var enable in toEnable)
            {
                enable.Enabled = !onOff;
            }
        }

        private BigO GetLambda(Assembly assembly)
        {
            var a = assembly.GetCustomAttribute<LambdaAttribute>();
            return a == null ? BigO.Squared : a.Lambda;
        }

        private void DrawArrays(Barrier obj)
        {
            try
            {
                SuspendLayout();
                for (int i = 0; i < _sorters.Count; ++i)
                {
                    var sorter = _sorters[i];
                    var imc = DgvSorters.Rows[i].SingleImageCell();
                    imc.Value = sorter.Draw(_drawSize);
                }
                ResumeLayout();
            }
            catch (IndexOutOfRangeException)
            {
                // This happens when sometimes when quitting. Doesn't matter anyway.
            }
        }

        private void UpdateBitmapSize()
        {
            if (DgvSorters.RowCount > 0)
            {
                var im = DgvSorters.GetCellDisplayRectangle(DgvSorters.ColumnCount - 1, 0, false);
                _drawSize = im.Size;
                Redraw();
            }
            else
            {
                _drawSize = new Size();
            }
        }

        private void InputArrayChanged()
        {
            if (SortLength > 0)
            {
                Generator.Count = SortLength;
                _numbers = Generator.Numbers;
                Redraw();
            }
        }

        private void Redraw()
        {
            if (_drawSize.Width * _drawSize.Height > 0)
            {
                var bm = SorterBase.Draw(_numbers, _drawSize);
                foreach (DataGridViewRow row in DgvSorters.Rows)
                {
                    var img = row.SingleImageCell();
                    img.Value = bm;
                }
            }
        }

        private bool SortingCorrect(ISorter sorter)
        {
            var rand = new Random();
            int[] lengths = { 0, 1, 64, 65, 66, 67, 1023 };
            int length = -1;
            try
            {
                for (int j = 0; j < lengths.Length; ++j)
                {
                    length = lengths[j];
                    var nums = new int[length];
                    for (int i = 0; i < length; ++i)
                    {
                        nums[i] = rand.Next();
                    }
                    sorter.Numbers = nums;
                    sorter.Sort();
                }
                sorter.Reset();
            }
            catch (SorterBase.NotSortedException)
            {
                Trace.TraceWarning("{0} failed with length {1}.", sorter.Name, length);
                return false;
            }

            return true;
        }

        #endregion
    }
}
