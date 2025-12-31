using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using System.Text.RegularExpressions;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace EMI_Calculator.ViewModels;

[Activity(Label = "", MainLauncher =true, Theme = "@style/AppTheme")]
public class LoginActivity : AppCompatActivity
{
    EditText _usernameEditText = null!;
    EditText _passwordEditText = null!;
    TextView _errorTextView = null!;
    ProgressBar _loginProgress = null!;
    Button _loginButton = null!;
    Button _registerButton = null!;
    TextView _forgotPassword = null!;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Create your application here
        SetContentView(Resource.Layout.LoginActivity);

        var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar)
            ?? throw new InvalidOperationException("Toolbar not found.");

       

       
        toolbar.NavigationIcon?.SetTint(Android.Graphics.Color.White);

        // Wire UI controls
        _usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText)
            ?? throw new InvalidOperationException("usernameEditText not found.");
        _passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText)
            ?? throw new InvalidOperationException("passwordEditText not found.");
        _errorTextView = FindViewById<TextView>(Resource.Id.errorTextView)
            ?? throw new InvalidOperationException("errorTextView not found.");
        _loginProgress = FindViewById<ProgressBar>(Resource.Id.loginProgress)
            ?? throw new InvalidOperationException("loginProgress not found.");
        _loginButton = FindViewById<Button>(Resource.Id.loginButton)
            ?? throw new InvalidOperationException("loginButton not found.");
        _registerButton = FindViewById<Button>(Resource.Id.registerButton)
            ?? throw new InvalidOperationException("registerButton not found.");
        _forgotPassword = FindViewById<TextView>(Resource.Id.forgotPassword)
            ?? throw new InvalidOperationException("forgotPassword not found.");

        _loginProgress.Visibility = ViewStates.Gone;
        _errorTextView.Visibility = ViewStates.Gone;

        _usernameEditText.Text = "test@example.com";
        _passwordEditText.Text = "Password123";

        // Event handlers
        // Use a named async void handler so we can add try/catch and logging
        _loginButton.Click += OnLoginButtonClicked;
        _registerButton.Click += OnRegisterClicked;
        _forgotPassword.Click += OnForgotPasswordClicked;

        Log.Debug("LoginActivity", "OnCreate completed and handlers wired.");
    }

    // Named async void handler for better diagnostics
    async void OnLoginButtonClicked(object? sender, EventArgs e)
    {
        try
        {
            Log.Debug("LoginActivity", "Login button clicked.");
            await OnLoginClickedAsync();
        }
        catch (Exception ex)
        {
            // Log and show a toast so failures are visible
            Log.Error("LoginActivity", ex.ToString());
            try
            {
                var toast = Toast.MakeText(this, "Login error: " + ex.Message, ToastLength.Short);
                toast?.Show();
            }
            catch { /* swallow toast errors */ }
        }
    }

    async Task OnLoginClickedAsync()
    {
        _errorTextView.Visibility = ViewStates.Gone;

        var username = _usernameEditText.Text?.Trim() ?? string.Empty;
        var password = _passwordEditText.Text ?? string.Empty;

        if (string.IsNullOrEmpty(username))
        {
            ShowError("Please enter your email or username.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("Please enter your password.");
            return;
        }

        // Optionally validate email format (if you expect emails)
        if (username.Contains("@") && !IsEmail(username))
        {
            ShowError("Please enter a valid email address.");
            return;
        }

        ToggleUiEnabled(false);
        _loginProgress.Visibility = ViewStates.Visible;

        try
        {
            // Replace FakeAuthenticateAsync with your real authentication call
            var success = await FakeAuthenticateAsync(username, password);

            if (success)
            {
                var toast = Toast.MakeText(this, "Login successful", ToastLength.Short);
                if (toast != null)
                {
                    toast.Show();
                }

                // Navigate to main activity (adjust type/namespace if different)
                var intent = new Intent(this, typeof(HomeActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            }
            else
            {
                ShowError("Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            ShowError("Login failed: " + ex.Message);
        }
        finally
        {
            _loginProgress.Visibility = ViewStates.Gone;
            ToggleUiEnabled(true);
        }
    }

    void OnRegisterClicked(object? sender, EventArgs e)
    {
        var toast = Toast.MakeText(this, "Register clicked — implement registration flow", ToastLength.Short);
        if (toast != null)
        {
            toast.Show();
        }
        // Navigate to your registration activity if you have one
    }

    void OnForgotPasswordClicked(object? sender, EventArgs e)
    {
        var toast = Toast.MakeText(this, "Forgot password clicked — implement recovery flow", ToastLength.Short);
        if (toast != null)
        {
            toast.Show();
        }
        // Navigate to your password recovery flow
    }

    void ShowError(string message)
    {
        _errorTextView.Text = message;
        _errorTextView.Visibility = ViewStates.Visible;
    }

    void ToggleUiEnabled(bool enabled)
    {
        _usernameEditText.Enabled = enabled;
        _passwordEditText.Enabled = enabled;
        _loginButton.Enabled = enabled;
        _registerButton.Enabled = enabled;
        _forgotPassword.Enabled = enabled;
    }

    // Demo authentication - replace with real API call
    static Task<bool> FakeAuthenticateAsync(string username, string password)
    {
        return Task.Run(async () =>
        {
            await Task.Delay(1200); // simulate network latency
                                    // Example demo credential — do NOT use in production
            return username.Equals("test@example.com", StringComparison.OrdinalIgnoreCase)
                   && password == "Password123";
        });
    }

    static bool IsEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        try
        {
            return Regex.IsMatch(input,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
