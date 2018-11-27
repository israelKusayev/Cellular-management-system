
using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Logger;
using Common.Models;
using Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Dal.Dals
{
    public class LineDal
    {
        PackageDal _packageDbManager;

        LoggerManager _logger;

        public LineDal()
        {
            _packageDbManager = new PackageDal();
            _logger = new LoggerManager(new FileLogger(), "lineDal.txt");

        }

        public Line AddNewLine(Line newLine, int customerId)
        {
            Line existLine;//exists?
            try
            {
                using (var context = new CellularContext())
                {
                    Customer customer = context.CustomerTable.SingleOrDefault((c) => c.CustomerId == customerId);
                    existLine = LineNumberIsAvailable(newLine, context);
                    if (existLine == null)
                    {
                        newLine.Status = LineStatus.Used;
                        newLine.CreatedDate = DateTime.Now;
                        customer.Lines.Add(newLine);
                        context.SaveChanges();
                        Line addedLine = context.LinesTable.SingleOrDefault((l) => l.LineNumber == newLine.LineNumber && l.Status == LineStatus.Used);
                        return addedLine;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (existLine.CustomerId == customerId)
            {
                throw new AlreadyExistExeption("this customer", "Line");
            }
            else
            {
                throw new FoundLineExeption("other customer", "Line");
            }
        }

        public List<Line> GetCustomerLines(string customerIdCard)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    var customer = context.CustomerTable.Where(c => c.IdentityCard == customerIdCard && c.IsActive == true).Include(c => c.Lines).SingleOrDefault();
                    return customer.Lines.Where(l => l.Status == LineStatus.Used).ToList();
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        public Line DeactivateLine(int lineId)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    Line lineToEdit = context.LinesTable.SingleOrDefault((l) => l.LineId == lineId && l.Status == LineStatus.Used);

                    if (lineToEdit != null)
                    {
                        lineToEdit.Status = LineStatus.Removed;
                        lineToEdit.RemovedDate = DateTime.Now;
                        context.SaveChanges();
                    }

                    return lineToEdit;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        public Package AddPackageToLine(int lineId, Package newPackage)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    Package addedPackage = null;

                    Line foundLine = context.LinesTable.SingleOrDefault((l) => l.LineId == lineId && l.Status == LineStatus.Used);

                    if (foundLine != null)
                    {
                        if (newPackage.IsPackageTemplate)
                        {
                            Package existPackage = context.PackagesTable.SingleOrDefault((p) => p.PackageId == newPackage.PackageId);
                            existPackage.Lines.Add(foundLine);
                            if (existPackage != null)
                            {
                                addedPackage = existPackage;
                            }
                        }
                        else
                        {
                            if (newPackage != null)
                            {
                                foundLine.Package = newPackage;
                                addedPackage = newPackage;
                            }
                        }
                        context.SaveChanges();
                    }
                    return addedPackage;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        public Package RemovePackageFromLine(int lineId)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    Package removedPackage = null;

                    Line line = context.LinesTable.Where((l) => l.LineId == lineId && l.Status == LineStatus.Used).Include(x => x.Package).SingleOrDefault();

                    if (line != null)
                    {
                        removedPackage = line.Package;
                        line.Package = null;
                        context.SaveChanges();
                    }
                    return removedPackage;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        /// <summary>
        /// Check if line is available
        /// </summary>
        /// <returns>Return line if there is line used or stolen or blocked otherwise return null</returns>
        private Line LineNumberIsAvailable(Line newLine, CellularContext context)
        {
            return context.LinesTable.SingleOrDefault((l) => l.LineNumber == newLine.LineNumber
                                                 && (!(l.Status == LineStatus.Avaliable) && !(l.Status == LineStatus.Removed)));
        }
    }
}
