using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QL_tour_LTW.ModelQLTOUR;
using ZXing;
using Microsoft.Reporting.WinForms;
using CustomControls.RJControls;
using System.Globalization;

namespace QL_tour_LTW
{
    public partial class formQUENMATKHAU : Form
    {
        private string correctCaptcha;
        private Result result;
        public formQUENMATKHAU()
        {
            InitializeComponent();
        }

        private void formQUENMATKHAU_Load(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }
        private void GenerateCaptcha()
        {
            // Tạo CAPTCHA ngẫu nhiên
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            correctCaptcha = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            // ảnh QR
            
        }
        private void SendEmail(string toEmail, string subject, string body)
        {
            string fromEmail = ""; // Thay thế bằng địa chỉ email của bạn
            string password = ""; // Thay thế bằng mật khẩu B2 email của bạn

            MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(fromEmail, password);

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi gửi email: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bntcapcha_Click(object sender, EventArgs e)
        {
            // đọc mã QR ra kết quả result
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            Bitmap qrCodeBitmap = writer.Write(correctCaptcha);
            pictureBoxQR.Image = qrCodeBitmap;
            BarcodeReader reader = new BarcodeReader();
            result = reader.Decode((Bitmap)pictureBoxQR.Image);
            //
            string email = txtTKNV.Texts;
        //    string password = GetPasswordFromDatabase(email); // Thay thế hàm này bằng cách lấy mật khẩu từ cơ sở dữ liệu của bạn
            string macapcha = correctCaptcha;
                try
                {
                    SendEmail(email, "Tourist Managerment System HTB change password", $"Mã xác thực thay đổi mật khẩu là: {macapcha}");
                    MessageBox.Show("Mật khẩu đã được gửi đến địa chỉ email của bạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi gửi email: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }            
        }

        private void btnHUY_Click(object sender, EventArgs e)
        {
            Close();
        }
        private bool checkxacnhanmk()
        {
            if (txtMATKHAUMOI.Texts == txtXACNHANMATKHAU.Texts)
            {
                return true;
            }
            return false;
        }
        private void reset()
        {
            txtXACNHANMATKHAU.Texts = txtMATKHAUMOI.Texts = txtTKNV.Texts = txtcapcha.Texts = string.Empty;
        }
        private int checktxt()
        {
            if (txtTKNV.Texts == "" || txtMATKHAUMOI.Texts == "" || txtXACNHANMATKHAU.Texts == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 1;
            }
            return 4;
        }
        private bool checkmkcu()
        {
            QLTOURDBContext context = new QLTOURDBContext();
            TKUSER capnhat = context.TKUSERs.FirstOrDefault(s => s.TENTAIKHOAN == txtTKNV.Texts && s.MATKHAU == txtMATKHAUMOI.Texts && s.VAITRO ==null);
            if (capnhat != null)
            {
                return false;
            }
            return true;
        }
        private bool checkemailsdt()
        {
            QLTOURDBContext context = new QLTOURDBContext();
            TKUSER email = context.TKUSERs.FirstOrDefault(s => s.TENTAIKHOAN == txtTKNV.Texts);
            if (email != null)
            {
                return true;
            }
            return false;
        }
        private void btnLUU_Click(object sender, EventArgs e)
        {
            int check = checktxt();
            if (check == 4)
            {
                if(checkemailsdt() == true)
                {
                    if (checkmkcu() == true)
                    {
                        if (checkxacnhanmk() == true)
                        {
                            if(txtcapcha.Texts == correctCaptcha || txtcapcha.Texts == result?.Text)
                            {
                                QLTOURDBContext context = new QLTOURDBContext();
                                TKUSER capnhat = context.TKUSERs.FirstOrDefault(s => s.TENTAIKHOAN == txtTKNV.Texts);
                                capnhat.TENTAIKHOAN = txtTKNV.Texts;
                                capnhat.MATKHAU = txtMATKHAUMOI.Texts;
                                capnhat.VAITRO = null;
                                capnhat.ANH = null;
                                context.SaveChanges();
                                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Mã xác thực sai!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtcapcha.Select();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Xác nhận mật khẩu không chính xác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtXACNHANMATKHAU.Select();
                            return;
                        }

                    }
                    else
                    {
                        MessageBox.Show("hãy đặt mật khẩu mới khác mật khẩu cũ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMATKHAUMOI.Select();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Tên tài khoản không đúng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTKNV.Select();
                    return;
                }
                
            }
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txtTKNV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 22) // 22 là mã ASCII của ký tự Ctrl + V
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true; // Loại bỏ ký tự khoảng trắng
            }
        }
        private bool IsDiacritic(char c)
        {
            string normalizedText = c.ToString().Normalize(NormalizationForm.FormD);
            foreach (char ch in normalizedText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                {
                    return true;
                }
            }
            return false;
        }
        private void txtXACNHANMATKHAU_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 22) // 22 là mã ASCII của ký tự Ctrl + V
            {
                e.Handled = true;
                return;
            }
            if (IsDiacritic(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnLUU_MouseDown(object sender, MouseEventArgs e)
        {
            btnLUU.BorderSize = 2;
            btnLUU.BorderColor = Color.MidnightBlue;
        }

        private void btnLUU_MouseUp(object sender, MouseEventArgs e)
        {
            btnLUU.BorderSize = 0;
        }

        private void btnHUY_MouseDown(object sender, MouseEventArgs e)
        {
            btnHUY.BorderSize = 2;
            btnHUY.BorderColor = Color.MidnightBlue;
        }

        private void btnHUY_MouseUp(object sender, MouseEventArgs e)
        {
            btnHUY.BorderSize = 0;
        }

        private void txtTKNV_Enter(object sender, EventArgs e)
        {
            txtTKNV.BackColor = Color.Gainsboro;
        }

        private void txtTKNV_Leave(object sender, EventArgs e)
        {
            txtTKNV.BackColor = Color.White;
        }

        private void txtMATKHAUMOI_Enter(object sender, EventArgs e)
        {
            txtMATKHAUMOI.BackColor = Color.Gainsboro;
        }

        private void txtMATKHAUMOI_Leave(object sender, EventArgs e)
        {
            txtMATKHAUMOI.BackColor = Color.White;

        }

        private void txtXACNHANMATKHAU_Enter(object sender, EventArgs e)
        {
            txtXACNHANMATKHAU.BackColor = Color.Gainsboro;
        }

        private void txtXACNHANMATKHAU_Leave(object sender, EventArgs e)
        {
            txtXACNHANMATKHAU.BackColor = Color.White;
        }

        private void txtcapcha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsDiacritic(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
