using Caliburn.Micro;
using KiscoSchedule.Database.Services;
using KiscoSchedule.Services;
using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KiscoSchedule.ViewModels
{
    class ShiftsViewModel : Screen
    {
        private IDatabaseService _databaseService;
        private IEventAggregator _events;
        private IUser _user;
        private ObservableCollection<IShift> shifts;
        private Shift selectedShift;
        private bool dataGridCurrentlyUpdating;

        public ShiftsViewModel(IDatabaseService databaseHelper, IEventAggregator events, IUser user)
        {
            _databaseService = databaseHelper;
            _events = events;
            _user = user;

            loadShifts();
        }

        private async void loadShifts()
        {
            shifts = new ObservableCollection<IShift>(await _databaseService.GetShiftsAsync(_user));
        }

        /// <summary>
        /// List of the shifts
        /// </summary>
        public ObservableCollection<IShift> Shifts
        {
            get
            {
                return shifts;
            }
            set
            {
                shifts = value;
                NotifyOfPropertyChange(() => Shifts);
            }
        }

        /// <summary>
        /// Callback for the add button
        /// </summary>
        public async void Add()
        {
            Shift shift = new Shift
            {
                Name = "New Shift"
            };

            long id = await _databaseService.CreateShiftAsync(_user, shift);
            shift.Id = id;

            Shifts.Add(shift);

            SelectedShift = shift;
        }
        
        /// <summary>
        /// Removes the selected shift
        /// </summary>
        public async void Remove()
        {
            if (SelectedShift == null)
                return;

            await _databaseService.RemoveShiftsAsync(SelectedShift);
            Shifts.Remove(SelectedShift);
        }

        /// <summary>
        /// The current selected shift
        /// </summary>
        public Shift SelectedShift
        {
            get { return selectedShift; }
            set
            {
                selectedShift = value;
                NotifyOfPropertyChange(() => SelectedShift);
            }
        }

        /// <summary>
        /// Event for editing the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="employeeObj"></param>
        /// <param name="e"></param>
        public async void DataGrid_RowEditEnding(object sender, object employeeObj, DataGridRowEditEndingEventArgs e)
        {
            if (sender == null)
                return;

            if (!dataGridCurrentlyUpdating)
            {
                dataGridCurrentlyUpdating = true;
                DataGrid dataGrid = (DataGrid)sender;

                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                dataGrid.Items.Refresh();
                dataGridCurrentlyUpdating = false;
            }

            if (e.EditAction == DataGridEditAction.Commit)
            {
                IShift shift = (Shift)employeeObj;

                await _databaseService.UpdateShiftAsync(shift);
            }
        }
    }
}
