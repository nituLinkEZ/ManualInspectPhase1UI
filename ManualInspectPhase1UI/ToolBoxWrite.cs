using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolBox
{
    public class ToolBoxWrite
    {
        public static bool IS_FORM_LOADED = false;
        public enum Property
        {
            Enable = 0,
            Visible = 1
        };


        public static void TextBoxWrite(TextBox tb, string data)
        {
            if(IS_FORM_LOADED)
            {
                tb.Invoke((Action)delegate
                {
                    tb.Text = data;
                    tb.Refresh();
                });
            }
            else
            {
                tb.Text = data;
                tb.Refresh();
            }
        }
        public static void ButtonWrite(Button btn,string data,Color color)
        {
            if (IS_FORM_LOADED)
            {
                btn.Invoke((Action)delegate
                {
                    btn.Text = data;
                    btn.ForeColor = color;
                    btn.Refresh();
                });
            }
            else
            {
                btn.Text = data;
                btn.ForeColor = color;
                btn.Refresh();
            }
        }
        public static void Button(Button[] btnArray, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                for (int i = 0; i < btnArray.Length; i++)
                {

                    btnArray[i].Invoke((Action)delegate
                    {
                        if (property == Property.Enable)
                        {
                            btnArray[i].Enabled = value;
                        }
                        else if (property == Property.Visible)
                        {
                            btnArray[i].Visible = value;
                        }
                        btnArray[i].Refresh();
                    });
                }
            }
            else
            {
                for (int i = 0; i < btnArray.Length; i++)
                {
                    if (property == Property.Enable)
                    {
                        btnArray[i].Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        btnArray[i].Visible = value;
                    }
                    btnArray[i].Refresh();
                }
            }
        }
        public static void Button(Button btn, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                btn.Invoke((Action)delegate
                {
                    if (property == Property.Enable)
                    {
                        btn.Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        btn.Visible = value;
                    }
                    btn.Refresh();
                });
            }
            else
            {
                if (property == Property.Enable)
                {
                    btn.Enabled = value;
                }
                else if (property == Property.Visible)
                {
                    btn.Visible = value;
                }
                btn.Refresh();
            }
        }

        public static void ComboBox(ComboBox[] cbArray, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                for (int i = 0; i < cbArray.Length; i++)
                {

                    cbArray[i].Invoke((Action)delegate
                    {
                        if (property == Property.Enable)
                        {
                            cbArray[i].Enabled = value;
                        }
                        else if (property == Property.Visible)
                        {
                            cbArray[i].Visible = value;
                        }
                        cbArray[i].Refresh();
                    });
                }
            }
            else
            {
                for (int i = 0; i < cbArray.Length; i++)
                {
                    if (property == Property.Enable)
                    {
                        cbArray[i].Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        cbArray[i].Visible = value;
                    }
                    cbArray[i].Refresh();
                }
            }
        }
        public static void ComboBox(ComboBox cb, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                cb.Invoke((Action)delegate
                {
                    if (property == Property.Enable)
                    {
                        cb.Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        cb.Visible = value;
                    }
                    cb.Refresh();
                });
            }
            else
            {
                if (property == Property.Enable)
                {
                    cb.Enabled = value;
                }
                else if (property == Property.Visible)
                {
                    cb.Visible = value;
                }
                cb.Refresh();
            }
        }

        public static void Label(Label lbl, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                lbl.Invoke((Action)delegate
                {
                    if (property == Property.Enable)
                    {
                        lbl.Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        lbl.Visible = value;
                    }
                    lbl.Refresh();
                });
            }
            else
            {
                if (property == Property.Enable)
                {
                    lbl.Enabled = value;
                }
                else if (property == Property.Visible)
                {
                    lbl.Visible = value;
                }
                lbl.Refresh();
            }
        }

        public static void Label(Label[] lblArray, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                for (int i = 0; i < lblArray.Length; i++)
                {

                    lblArray[i].Invoke((Action)delegate
                    {
                        if (property == Property.Enable)
                        {
                            lblArray[i].Enabled = value;
                        }
                        else if (property == Property.Visible)
                        {
                            lblArray[i].Visible = value;
                        }
                    });
                }
            }
            else
            {
                for (int i = 0; i < lblArray.Length; i++)
                {
                    if (property == Property.Enable)
                    {
                        lblArray[i].Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        lblArray[i].Visible = value;
                    }
                }
            }
        }

        public static void CheckBox(CheckBox cBox, bool value, Property property)
        {
            if(IS_FORM_LOADED)
            {
                cBox.Invoke((Action)delegate
                {
                    if(property == Property.Enable)
                    {
                        cBox.Enabled = value;
                    }
                    else if(property == Property.Visible)
                    {
                        cBox.Visible = value;
                    }
                    cBox.Refresh();
                });
            }
            else
            {
                if (property == Property.Enable)
                {
                    cBox.Enabled = value;
                }
                else if (property == Property.Visible)
                {
                    cBox.Visible = value;
                }
                cBox.Refresh();
            }
        }

        public static void ToolStripMenuItem(ToolStripMenuItem[] cBoxArray, bool value, Property property)
        {
            //if (IS_FORM_LOADED)
            //{
            //    for (int i = 0; i < cBoxArray.Length; i++)
            //    {
            //        cBoxArray[i].GetCurrentParent().Invoke((Action)delegate
            //        {
            //            if (property == Property.Enable)
            //            {
            //                cBoxArray[i].Enabled = value;
            //            }
            //            else if (property == Property.Visible)
            //            {
            //                cBoxArray[i].Visible = value;
            //            }
            //        });
            //    }
            //}
            //else
            {
                for (int i = 0; i < cBoxArray.Length; i++)
                {
                    if (property == Property.Enable)
                    {
                        cBoxArray[i].Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        cBoxArray[i].Visible = value;
                    }
                }
            }
        }
        public static void CheckBox(CheckBox[] cBoxArray, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                for (int i = 0; i < cBoxArray.Length; i++)
                {

                    cBoxArray[i].Invoke((Action)delegate
                    {
                        if (property == Property.Enable)
                        {
                            cBoxArray[i].Enabled = value;
                        }
                        else if (property == Property.Visible)
                        {
                            cBoxArray[i].Visible = value;
                        }
                        cBoxArray[i].Refresh();
                    });
                }
            }
            else
            {
                for (int i = 0; i < cBoxArray.Length; i++)
                {
                    if (property == Property.Enable)
                    {
                        cBoxArray[i].Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        cBoxArray[i].Visible = value;
                    }
                    cBoxArray[i].Refresh();
                }
            }
        }
        public static void TextBoxWrite(TextBox tb, string data, Color color)
        {
            if (IS_FORM_LOADED)
            {
                tb.Invoke((Action)delegate
                {
                    tb.ForeColor = color;
                    tb.Text = data;
                    tb.Refresh();
                });
            }
            else
            {
                tb.ForeColor = color;
                tb.Text = data;
                tb.Refresh();
            }
        }
        public static void TextBoxWrite(TextBox tb, string data,bool enable,bool visible)
        {
            if (IS_FORM_LOADED)
            {
                tb.Invoke((Action)delegate
                {
                    tb.Enabled = enable;
                    tb.Visible = visible;
                    tb.Text = data;
                    tb.Refresh();
                });
            }
            else
            {
                tb.Enabled = enable;
                tb.Visible = visible;
                tb.Text = data;
                tb.Refresh();
            }
        }
        public static void TextBox(TextBox[] tbArray, bool value, Property property)
        {
            if (IS_FORM_LOADED)
            {
                for (int i = 0; i < tbArray.Length; i++)
                {

                    tbArray[i].Invoke((Action)delegate
                    {
                        if (property == Property.Enable)
                        {
                            tbArray[i].Enabled = value;
                        }
                        else if (property == Property.Visible)
                        {
                            tbArray[i].Visible = value;
                        }
                    });
                }
            }
            else
            {
                for (int i = 0; i < tbArray.Length; i++)
                {
                    if (property == Property.Enable)
                    {
                        tbArray[i].Enabled = value;
                    }
                    else if (property == Property.Visible)
                    {
                        tbArray[i].Visible = value;
                    }
                }
            }
        }

 
        public static string TextBoxRead(TextBox tb)
        {
            string tbVal = null;
            if (IS_FORM_LOADED)
            {
                tb.Invoke((Action)delegate
                {
                    tbVal = tb.Text;
                });
            }
            else
            {
                tbVal = tb.Text;
            }
            return tbVal;
        }
        public static void LabelWrite(Label lbl, string data)
        {
            if (IS_FORM_LOADED)
            {
                lbl.Invoke((Action)delegate
                {
                    lbl.Text = data;
                    lbl.Refresh();
                });
            }
            else
            {
                lbl.Text = data;
                lbl.Refresh();
            }
        }
        public static void LabelWrite(Label lbl, string data, Color color)
        {
            if (IS_FORM_LOADED)
            {
                lbl.Invoke((Action)delegate
                {
                    lbl.ForeColor = color;
                    lbl.Text = data;
                    lbl.Refresh();
                });
            }
            else
            {
                lbl.ForeColor = color;
                lbl.Text = data;
                lbl.Refresh();
            }
        }
        public static string LabelRead(Label lbl)
        {
            string data = string.Empty;
            if (IS_FORM_LOADED)
            {
                lbl.Invoke((Action)delegate
                {
                    data = lbl.Text;
                });
            }
            else
            {
                data = lbl.Text;
            }
            return data;
        }
        public static void ComboBoxWrite(ComboBox cb, string data)
        {
            if (IS_FORM_LOADED)
            {
                cb.Invoke((Action)delegate
                {
                    cb.Text = data;
                });
            }
            else
            {
                cb.Text = data;
            }
        }
        public static void ComboBoxAdd(ComboBox cb, string[] itemList)
        {
            if (IS_FORM_LOADED)
            {
                cb.Invoke((Action)delegate
                {
                    foreach (string data in itemList)
                    {
                        cb.Text = data;
                    }
                });
            }
            else
            {
                foreach (string data in itemList)
                {
                    cb.Text = data;
                }
            }
        }
    }
}
