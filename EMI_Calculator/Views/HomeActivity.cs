using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace EMI_Calculator.ViewModels;

[Activity(Label = "", Theme = "@style/AppTheme")]
public class HomeActivity : AppCompatActivity   
{
    LinearLayout _calculateEMI= null!;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.HomeActivity);

        var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar)
            ?? throw new InvalidOperationException("Toolbar not found.");

        SetSupportActionBar(toolbar);

        SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
        SupportActionBar?.SetDisplayShowHomeEnabled(true);
        toolbar.NavigationIcon?.SetTint(Android.Graphics.Color.White);

        // Create your application here
        _calculateEMI = FindViewById<LinearLayout>(Resource.Id.calculateEMI)
            ?? throw new InvalidOperationException("calculateEMIButton not found.");

        // Event handlers
        _calculateEMI.Click += OnCalculateEMIClicked;
    }

    private void OnCalculateEMIClicked(object? sender, EventArgs e)
    {
        var intent = new Intent(this, typeof(CalculateEMIActivity));
        intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
        StartActivity(intent);
        //Finish();
    }
    public override bool OnOptionsItemSelected(IMenuItem item)
    {
        if (item.ItemId == Android.Resource.Id.Home)
        {
            OnBackPressed();
            return true;
        }
        return base.OnOptionsItemSelected(item);
    }
    public override void OnBackPressed()
    {
        MoveTaskToBack(true); // app goes to background
    }
}