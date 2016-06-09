using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using Akka.Util.Internal;
using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private IActorRef _chartActor;
        //private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);

        private IActorRef _coordinatorActor = null;
        private Dictionary<ChartingActor.CounterType, IActorRef> _toggleActors = new Dictionary<ChartingActor.CounterType, IActorRef>();

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization

        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
            _chartActor.Tell(new ChartingActor.InitializeChart(null)); // no initial series

            _coordinatorActor = Program.ChartActors.ActorOf(Props.Create(() => new PerformaceCounterCoordinatorActor(_chartActor)), "counters");

            Action<ChartingActor.CounterType, Button> setupButtonToggleActor = (counterType, btn) =>
                {
                    var props = Props.Create(() => new ButtonToggleActor(_coordinatorActor, btn, counterType, false))
                        // * This .WithDispatcher() as well as any other programmatic config setting will be overridden by a HOCON config setting if one is specified for this actor!
                        .WithDispatcher("akka.actor.synchronized-dispatcher");
                    _toggleActors[counterType] = Program.ChartActors.ActorOf(props);
                };

            setupButtonToggleActor(ChartingActor.CounterType.Cpu, cpuBtn);
            setupButtonToggleActor(ChartingActor.CounterType.Memory, memoryBtn);
            setupButtonToggleActor(ChartingActor.CounterType.Disk, diskBtn);

            // Set the CPU toggle to ON so we start getting some data
            _toggleActors[ChartingActor.CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Shutdown();
        }

        #endregion

        private void cpuBtn_Click(object sender, EventArgs e)
        {
            _toggleActors[ChartingActor.CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
        }

        private void memoryBtn_Click(object sender, EventArgs e)
        {
            _toggleActors[ChartingActor.CounterType.Memory].Tell(new ButtonToggleActor.Toggle());
        }

        private void diskBtn_Click(object sender, EventArgs e)
        {
            _toggleActors[ChartingActor.CounterType.Disk].Tell(new ButtonToggleActor.Toggle());
        }
    }
}