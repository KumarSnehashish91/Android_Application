using Android.Views;
using AndroidX.AppCompat.App;
using EMI_Calculator.Models;
using Java.Lang;
using Math = System.Math;
using AndroidX.AppCompat.Widget;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace EMI_Calculator.ViewModels;

[Activity(Label = "", Theme = "@style/AppTheme")]
public class CalculateEMIActivity : AppCompatActivity
{
    EditText _txtLoanAmt = null!;
    EditText _txtInterestPer = null!;
    EditText _txtPeriod = null!;
    EditText _txtEMI = null!;
    EditText _txtProcessFee = null!;
    Button _btnCalc = null!;
    Button _btnReset = null!;
    LinearLayout _rgFields = null!;
    RadioButton rbLoanAmt= null!;
    RadioButton rbInterestPer = null!;
    RadioButton rbPeriod = null!;
    RadioButton rbEMI = null!;
    TextView _txtResultEmi = null!;
    TextView _txtResultInterest = null!;
    TextView _txtResultProcessingFee = null!;
    TextView _txtResultTotal = null!;
    LinearLayout _layoutResult= null!;


    double loanAmt;
    double interest;
    double period;
    double emi;
    double processFee;
    
    public CalculateEMIViewModel? _viewModel;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.CalculateEMIActivity);

        var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar)
            ?? throw new InvalidOperationException("Toolbar not found.");

        SetSupportActionBar(toolbar);

        SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
        SupportActionBar?.SetDisplayShowHomeEnabled(true);
        toolbar.NavigationIcon?.SetTint(Android.Graphics.Color.White);

        _viewModel = new CalculateEMIViewModel();

        // Create your application here
        _txtLoanAmt = FindViewById<EditText>(Resource.Id.txtLoanAmt)
            ?? throw new InvalidOperationException("txtLoanAmt not found.");

        _txtInterestPer = FindViewById<EditText>(Resource.Id.txtInterestPer)
            ?? throw new InvalidOperationException("txtInterestPer not found.");

        _txtPeriod = FindViewById<EditText>(Resource.Id.txtPeriod)
            ?? throw new InvalidOperationException("txtPeriod not found.");

        _txtEMI = FindViewById<EditText>(Resource.Id.txtEMI)
            ?? throw new InvalidOperationException("txtEMI not found.");

        _txtProcessFee = FindViewById<EditText>(Resource.Id.txtProcessFee)
            ?? throw new InvalidOperationException("txtProcessFee not found.");

        _btnCalc = FindViewById<Button>(Resource.Id.btnCalc)
            ?? throw new InvalidOperationException("btnCalc not found.");

        _btnReset = FindViewById<Button>(Resource.Id.btnReset)
            ?? throw new InvalidOperationException("btnReset not found.");
        
        rbLoanAmt= FindViewById< RadioButton>(Resource.Id.rbLoanAmt)
            ?? throw new InvalidOperationException("rbLoanAmt not found.");

        rbInterestPer = FindViewById<RadioButton>(Resource.Id.rbInterestPer)
            ?? throw new InvalidOperationException("rbInterestPer not found.");

        rbPeriod = FindViewById<RadioButton>(Resource.Id.rbPeriod)
            ?? throw new InvalidOperationException("rbPeriod not found.");

        rbEMI = FindViewById<RadioButton>(Resource.Id.rbEMI)
            ?? throw new InvalidOperationException("rbEMI not found.");

       

        // RadioGroup now contains RadioButtons as direct children (XML was changed).
        _rgFields = FindViewById<LinearLayout>(Resource.Id.rgFields)
            ?? throw new InvalidOperationException("RadioGroup rgFields not found.");

        _txtResultEmi = FindViewById<TextView>(Resource.Id.txtResultEmi)
            ?? throw new InvalidOperationException("txtResultEmi not found.");

        _txtResultInterest = FindViewById<TextView>(Resource.Id.txtResultInterest)
            ?? throw new InvalidOperationException("txtResultInterest not found.");

        _txtResultProcessingFee = FindViewById<TextView>(Resource.Id.txtResultProcessingFee)
            ?? throw new InvalidOperationException("txtResultProcessingFee not found.");

        _txtResultTotal = FindViewById<TextView>(Resource.Id.txtResultTotal)
            ?? throw new InvalidOperationException("txtResultTotal not found.");

        _layoutResult = FindViewById<LinearLayout>(Resource.Id.layoutResult)
            ?? throw new InvalidOperationException("layoutResult not found.");

        SetupExclusiveRadioButtons();        

        // Event handlers
        _btnCalc.Click += (sender, e) =>
        {
            var input = new EmiInput
            {
                LoanAmount = ParseDecimal(_txtLoanAmt),
                InterestRate = ParseDecimal(_txtInterestPer),
                PeriodInYears = (int)ParseDecimal(_txtPeriod),
                Emi = ParseDecimal(_txtEMI)
            };

            try
            {
                decimal result = _viewModel.Calculate(input);
                DisplayResult(result);

                if (_viewModel.SelectedCalculation == CalculationType.Emi)
                {
                    ShowResultTable(result, input);
                }
                
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        };
        _btnReset.Click += OnBtnResetClicked;
    }
        
    private void OnBtnResetClicked(object? sender, EventArgs e)
    {
        // Clear inputs and radio selection
        _txtLoanAmt.Text = string.Empty;
        _txtInterestPer.Text = string.Empty;
        _txtPeriod.Text = string.Empty;
        _txtEMI.Text = string.Empty;
        _txtProcessFee.Text = string.Empty;

        //_rgFields.ClearCheck();
    }
    void SetupExclusiveRadioButtons()
    {
        RadioButton[] radios =
        {
        rbLoanAmt,
        rbInterestPer,
        rbPeriod,
        rbEMI        
    };

        foreach (var rb in radios)
        {
            rb.CheckedChange += (s, e) =>
            {
                if (!e.IsChecked) return;

                foreach (var other in radios)
                {
                    if (other != rb)
                        other.Checked = false;
                }
                _viewModel.SelectedCalculation = GetCalculationType(rb);
                ApplyFieldRules(_viewModel.SelectedCalculation);
            };
        }
    }
    private void ApplyFieldRules(CalculationType type)
    {
        _txtLoanAmt.Enabled = false;
        _txtInterestPer.Enabled = false;
        _txtPeriod.Enabled = false;
        _txtEMI.Enabled = false;
        _txtProcessFee.Enabled = false;

        switch (type)
        {
            case CalculationType.Emi:
                // EMI = f(Loan, Interest, Period)
                _txtLoanAmt.Enabled = true;
                _txtInterestPer.Enabled = true;
                _txtPeriod.Enabled = true;
                _txtEMI.Enabled = false; // OUTPUT
                break;

            case CalculationType.LoanAmount:
                // Loan = f(EMI, Interest, Period)
                _txtEMI.Enabled = true;
                _txtInterestPer.Enabled = true;
                _txtPeriod.Enabled = true;
                _txtLoanAmt.Enabled = false; // OUTPUT
                break;

            case CalculationType.Interest:
                // Interest = f(Loan, EMI, Period)
                _txtLoanAmt.Enabled = true;
                _txtEMI.Enabled = true;
                _txtPeriod.Enabled = true;
                _txtInterestPer.Enabled = false; // OUTPUT
                break;

            case CalculationType.Period:
                // Period = f(Loan, EMI, Interest)
                _txtLoanAmt.Enabled = true;
                _txtEMI.Enabled = true;
                _txtInterestPer.Enabled = true;
                _txtPeriod.Enabled = false; // OUTPUT
                break;

            case CalculationType.ProcessFee:
                // Processing fee usually depends on Loan Amount
                _txtLoanAmt.Enabled = true;
                _txtProcessFee.Enabled = false; // OUTPUT
                break;

            case CalculationType.None:
            default:
                // Nothing selected → keep everything disabled
                break;
        }
    }
    private CalculationType GetCalculationType(RadioButton rb)
    {
        if (rb == rbLoanAmt) return CalculationType.LoanAmount;
        if (rb == rbInterestPer) return CalculationType.Interest;
        if (rb == rbPeriod) return CalculationType.Period;
        if (rb == rbEMI) return CalculationType.Emi;

        return CalculationType.None;
    }
    private decimal ParseDecimal(EditText editText)
    {
        if (editText == null)
            return 0m;

        return decimal.TryParse(editText.Text, out var value)
            ? value
            : 0m;
    }
    private void ShowResultTable(decimal emi, EmiInput input)
    {
        int months = input.PeriodInYears * 12;

        decimal totalPayment = emi * months;
        decimal totalInterest = totalPayment - input.LoanAmount;

        decimal processingFee =
            input.LoanAmount * ParseDecimal(_txtProcessFee) / 100m;

        decimal grandTotal =
            input.LoanAmount + totalInterest + processingFee;

        _txtResultEmi.Text = FormatCurrency(emi);
        _txtResultInterest.Text = FormatCurrency(totalInterest);
        _txtResultProcessingFee.Text = FormatCurrency(processingFee);
        _txtResultTotal.Text = FormatCurrency(grandTotal);

        _layoutResult.Visibility = ViewStates.Visible;
    }
    private string FormatCurrency(decimal value)
    {
        return string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-IN"),
            "{0:N0}", value);
    }
    private void DisplayResult(decimal result)
    {
        switch (_viewModel.SelectedCalculation)
        {
            case CalculationType.Emi:
                _txtEMI.Text = FormatCurrency(result);
                break;

            case CalculationType.LoanAmount:
                _txtLoanAmt.Text = FormatCurrency(result);
                break;

            case CalculationType.Interest:
                _txtInterestPer.Text = result.ToString("0.##");
                break;

            case CalculationType.Period:
                _txtPeriod.Text = result.ToString("0");
                break;

            case CalculationType.ProcessFee:
                _txtProcessFee.Text = FormatCurrency(result);
                break;

            default:
                // No-op
                break;
        }
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
        Finish(); // simply close current screen
    }

}