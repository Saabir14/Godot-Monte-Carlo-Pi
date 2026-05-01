using Godot;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public partial class Tester : Node
{
    [Export] private MonteCarlo monte_carlo_script = new MonteCarlo();
    private LineEdit input;
    private BaseButton button;
    private Label text;
    private ProgressBar progress;
    public override void _Ready()
    {
        input = GetNode<LineEdit>("LineEdit");
        button = GetNode<Button>("Button");
        text = GetNode<Label>("Label");
        progress = GetNode<ProgressBar>("Range");

        button.Connect(BaseButton.SignalName.Pressed, Callable.From(() => {
            Task task = new Task(() => {
                // Disable user input to prevent spam
                button.SetDeferred(BaseButton.PropertyName.Disabled, true);

                // Get parameter
                int interval = input.Text.ToInt();

                // Set progress bar max
                progress.SetDeferred(Range.PropertyName.MaxValue, interval);

                // Start watch
                var clock = Stopwatch.StartNew();

                // Get value
                var pi = monte_carlo_script.estimatePi(interval);

                // Stop watch
                clock.Stop();
                
                // Show findings
                text.SetDeferred(Label.PropertyName.Text, "Calculated Pi: " + pi + "\nTook " + clock.ElapsedMilliseconds + " ms");

                // Enable user input again
                button.SetDeferred(BaseButton.PropertyName.Disabled, false);
            });
            task.Start();
        }));
    }

    public override void _Process(double delta)
    {
        progress.Indeterminate = monte_carlo_script.progress == -1;
        progress.Value = monte_carlo_script.progress;
    }
}
