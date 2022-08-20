using System;
using System.Linq;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditEntryPage : ContentPage
    {
        private readonly IOtpFormContext _formContext;

        public EditEntryPage(IOtpFormContext formContext)
        {
            InitializeComponent();

            _formContext = formContext;

            submitButton.Text = formContext.SubmitText;
            
            LoadFormData(_formContext.GetDefault());
        }
    
        private void LoadFormData(OtpData defaultData)
        {
            // Label
            labelEditor.Text = defaultData.Label;

            // Issuer
            issuerEditor.Text = defaultData.Issuer;

            // Secret
            secretEditor.Text = defaultData.Secret;

            // Algorithms
            foreach (string algo in TotpUtilities.SUPPORTED_ALGORITHMS)
                algorithmPicker.Items.Add(algo);
            algorithmPicker.SelectedIndex = Array.IndexOf(TotpUtilities.SUPPORTED_ALGORITHMS, defaultData.Algorithm);

            // Digits
            foreach (int value in TotpUtilities.SUPPORTED_DIGIT_COUNTS)
                digitsPicker.Items.Add(value.ToString());
            digitsPicker.SelectedIndex = Array.IndexOf(TotpUtilities.SUPPORTED_DIGIT_COUNTS, defaultData.Digits);

            // Period
            foreach (string value in TotpUtilities.SUPPORTED_PERIOD_NAMES)
                periodPicker.Items.Add(value);
            periodPicker.SelectedIndex = Array.IndexOf(TotpUtilities.SUPPORTED_PERIOD_VALUES, defaultData.Period);

            // Type
            foreach (string value in TotpUtilities.TYPE_NAMES)
                typePicker.Items.Add(value);
            typePicker.SelectedIndex = Array.IndexOf(TotpUtilities.TYPE_VALUES, defaultData.Type);

            // Counter
            counterEditor.Text = defaultData.Counter.ToString();
        }

        private bool TryGetFormData(out OtpData otpData, ref string message)
        {
            otpData = null;

            // Label
            string label = labelEditor.Text;
            if (string.IsNullOrWhiteSpace(label))
                return false;

            // Secret
            string secret = secretEditor.Text;
            if (string.IsNullOrWhiteSpace(secret) || !TotpUtilities.ValidateSecret(secret))
            {
                message = "Invalid scret: expected a base-32 encoded value";
                return false;
            }

            // Issuer
            string issuer = issuerEditor.Text;
            if (string.IsNullOrWhiteSpace(issuer))
                return false;

            // Algorithm
            if (!(algorithmPicker.SelectedItem is string algorithm) ||
                !TotpUtilities.SUPPORTED_ALGORITHMS.Contains(algorithm))
                return false;

            // Digits
            if (!(digitsPicker.SelectedItem is string digitsName) ||
                !int.TryParse(digitsName, out int digits) ||
                !TotpUtilities.SUPPORTED_DIGIT_COUNTS.Contains(digits))
                return false;

            // Period
            if (!(periodPicker.SelectedItem is string periodName) ||
                !TotpUtilities.TryParsePeriodName(periodName, out int period))
                return false;

            // Type
            if (!(typePicker.SelectedItem is string typeName) ||
                !TotpUtilities.TryParseTypeName(typeName, out OtpType type))
                return false;

            // Counter
            int counter = 0;
            if (type == OtpType.Hotp)
            {
                if (!int.TryParse(counterEditor.Text, out counter))
                    return false;
            }

            otpData = new OtpData(type, label, secret, issuer, algorithm, digits, counter, period);
            return true;
        }

        private void ShowAdvancedOptions(object sender, EventArgs e)
        {
            advancedOptionsContainer.IsVisible = true;
            showAdvancedOptionsContainer.IsVisible = false;
        }

        private void TypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isHotp = typePicker.SelectedItem is string typeName &&
                          TotpUtilities.TryParseTypeName(typeName, out OtpType type) &&
                          type == OtpType.Hotp;

            counterLabel.IsVisible = isHotp;
            counterEditor.IsVisible = isHotp;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            string message = null;
            if (TryGetFormData(out OtpData data, ref message))
            {                
                _formContext.Set(data);
                await Navigation.PopAsync();
            }
            else if (message != null)
            {
                await DisplayAlert("Invalid information", message, "OK");
            }
        }
    }
}