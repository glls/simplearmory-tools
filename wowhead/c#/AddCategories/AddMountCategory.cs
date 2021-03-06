﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WowheadParser
{
    public partial class AddMountCategory : Form, IAddCategory
    {
        public IParser Parser { get; private set; }
        private MountParser mp;

        public AddMountCategory(IParser parser)
        {
            InitializeComponent();

            // Clone the parser buy using its json
            this.Parser = WowheadParser.Parser.Create(parser.JsonFile, parser.ParseType);
            this.mp = (MountParser)this.Parser;

            // Fill out the cats list view
            foreach (var cat in mp.Cats)
            {
                this.categoryListBox.Items.Add(cat);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
     
        private void categoryListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            this.zoneListBox.BeginUpdate();
            this.zoneListBox.Items.Clear();

            if (this.categoryListBox.SelectedItems.Count > 0)
            {
                foreach (var sc in ((MountCategory) this.categoryListBox.SelectedItem).subcats)
                {
                    this.zoneListBox.Items.Add(sc);
                }
            }

            this.zoneListBox.EndUpdate();
            this.zoneRemove.Enabled = false;

            this.categoryRemove.Enabled = this.categoryListBox.SelectedItems.Count > 0;
            this.downCat.Enabled = this.categoryListBox.SelectedItems.Count > 0 && this.categoryListBox.SelectedIndex != (this.categoryListBox.Items.Count - 1);
            this.upCat.Enabled = this.categoryListBox.SelectedItems.Count > 0 && this.categoryListBox.SelectedIndex != 0;
        }

        private void zoneListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            this.zoneRemove.Enabled = this.zoneListBox.SelectedItems.Count > 0;
            this.downZone.Enabled = this.zoneListBox.SelectedItems.Count > 0 && this.zoneListBox.SelectedIndex != (this.zoneListBox.Items.Count - 1);
            this.upZone.Enabled = this.zoneListBox.SelectedItems.Count > 0 && this.zoneListBox.SelectedIndex != 0;
        }

        private void zoneRemove_Click(object sender, EventArgs e)
        {
            ((MountCategory) this.categoryListBox.SelectedItem).subcats.Remove((MountSubCat) this.zoneListBox.SelectedItem);
            this.zoneListBox.Items.Remove(this.zoneListBox.SelectedItem);
        }

        private void categoryRemove_Click(object sender, EventArgs e)
        {          
            this.mp.Cats.Remove((MountCategory)this.categoryListBox.SelectedItem);
            this.categoryListBox.Items.Remove(this.categoryListBox.SelectedItem);
            this.zoneListBox.Items.Clear();
        }

        private void categoryAdd_Click(object sender, EventArgs e)
        {
            var inform = new Prompt("Category");
            if (inform.ShowDialog() == DialogResult.OK)
            {
                var newCategory = new MountCategory()
                {
                    subcats = new List<MountSubCat>(),
                    name = inform.Value
                };

                this.mp.Cats.Add(newCategory);
                this.categoryListBox.Items.Add(newCategory);
            }
        }

        private void zoneAdd_Click(object sender, EventArgs e)
        {
            var inform = new Prompt("SubCat");
            if (inform.ShowDialog() == DialogResult.OK)
            {
                var newSubCat = new MountSubCat()
                {
                    items = new List<Mount>(),
                    name = inform.Value
                };

                ((MountCategory)this.categoryListBox.SelectedItem).subcats.Add(newSubCat);
                this.zoneListBox.Items.Add(newSubCat);
            }
        }
      
        private void upCat_Click(object sender, EventArgs e)
        {
            // move selected item up        
            var removeIndex = this.categoryListBox.SelectedIndex + 1;
            var addIndex = this.categoryListBox.SelectedIndex - 1;
            var item = this.categoryListBox.SelectedItem;
            this.categoryListBox.Items.Insert(addIndex, item);
            this.categoryListBox.Items.RemoveAt(removeIndex);
            this.categoryListBox.SelectedIndex = addIndex;

            // move it up in mem too
            this.mp.Cats.Insert(addIndex, (MountCategory)item);
            this.mp.Cats.RemoveAt(removeIndex);
        }

        private void downCat_Click(object sender, EventArgs e)
        {
            var removeIndex = this.categoryListBox.SelectedIndex;
            var addIndex = this.categoryListBox.SelectedIndex + 2;
            var item = this.categoryListBox.SelectedItem;
            this.categoryListBox.Items.Insert(addIndex, item);
            this.categoryListBox.Items.RemoveAt(removeIndex);
            this.categoryListBox.SelectedIndex = addIndex - 1;

            // move it up in mem too
            this.mp.Cats.Insert(addIndex, (MountCategory)item);
            this.mp.Cats.RemoveAt(removeIndex);
        }

        private void upZone_Click(object sender, EventArgs e)
        {
            // move selected item up        
            var removeIndex = this.zoneListBox.SelectedIndex + 1;
            var addIndex = this.zoneListBox.SelectedIndex - 1;
            var item = this.zoneListBox.SelectedItem;
            this.zoneListBox.Items.Insert(addIndex, item);
            this.zoneListBox.Items.RemoveAt(removeIndex);
            this.zoneListBox.SelectedIndex = addIndex;

            // move it up in mem too
            ((MountCategory)this.categoryListBox.SelectedItem).subcats.Insert(addIndex, (MountSubCat)item);
            ((MountCategory)this.categoryListBox.SelectedItem).subcats.RemoveAt(removeIndex);
        }

        private void downZone_Click(object sender, EventArgs e)
        {
            var removeIndex = this.zoneListBox.SelectedIndex;
            var addIndex = this.zoneListBox.SelectedIndex + 2;
            var item = this.zoneListBox.SelectedItem;
            this.zoneListBox.Items.Insert(addIndex, item);
            this.zoneListBox.Items.RemoveAt(removeIndex);
            this.zoneListBox.SelectedIndex = addIndex - 1;

            // move it up in mem too
            ((MountCategory)this.categoryListBox.SelectedItem).subcats.Insert(addIndex, (MountSubCat)item);
            ((MountCategory)this.categoryListBox.SelectedItem).subcats.RemoveAt(removeIndex);
        }        
    }
}
