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
    public class PackageDal
    {
        LoggerManager _logger;

        public PackageDal()
        {
            _logger = new LoggerManager(new FileLogger(), "packageDal.txt");
        }

        /// <summary>
        /// Get all package templates
        /// </summary>
        /// <returns>List of packages if at least one exists otherwise null</returns>
        public List<Package> GetPackageTemplates()
        {
            try
            {
                using (var context = new CellularContext())
                {
                    List<Package> packages = context.PackagesTable.Where((p) => p.IsPackageTemplate == true).ToList();
                    return packages;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        /// <summary>
        /// Get package include friends if friends exists
        /// </summary>
        /// <param name="lineId"> Line id</param>
        /// <returns>Package if exists to given line otherwise null</returns>
        public Package GetPackage(int lineId)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    Line line = context.LinesTable.Where((l) => l.LineId == lineId).Include(x => x.Package).Include(y => y.Package.Friends).SingleOrDefault();

                    if (line != null)
                    {
                        return line.Package;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        /// <summary>
        /// Edit package details
        /// </summary>
        /// <param name="packageId">Package id</param>
        /// <param name="lineId">Line id</param>
        /// <param name="packageToEdit">Edited package</param>
        /// <returns>Edited package if package found, otherwise null</returns>
        public Package EditPackage(int packageId, int lineId, Package packageToEdit)
        {
            Package EditedPackage = null;

            try
            {
                using (var context = new CellularContext())
                {
                    if (packageToEdit.IsPackageTemplate)
                    {
                        EditedPackage = context.PackagesTable.SingleOrDefault((p) => p.PackageId == packageToEdit.PackageId);
                        if (EditedPackage != null)
                        {
                            Line line = context.LinesTable.SingleOrDefault(l => l.LineId == lineId);
                            EditedPackage.Lines.Add(line);
                        }
                    }
                    else
                    {
                        EditedPackage = context.PackagesTable.Where((p) => p.PackageId == packageId).Include(x => x.Friends).SingleOrDefault();
                        if (EditedPackage != null)
                        {
                            //EditedPackage = context.PackagesTable.SingleOrDefault((p) => p.PackageId == packageId);
                            if (EditedPackage != null)
                            {
                                packageToEdit.PackageId = EditedPackage.PackageId;
                                context.Entry(EditedPackage).CurrentValues.SetValues(packageToEdit);
                            }
                        }
                        context.SaveChanges();

                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            return EditedPackage;
        }

        /// <summary>
        /// Remove line from template package if the package is edited to custom package
        /// </summary>
        /// <param name="lineId">Line id</param>
        public void RemoveLineFromTemplatePackage(int lineId)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    Line line = context.LinesTable.Where((l) => l.LineId == lineId).Include(x => x.Package).SingleOrDefault();
                    Package package = line.Package;
                    if (package.IsPackageTemplate)
                    {
                        if (line != null)
                        {
                            package.Lines.Remove(line);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        /// <summary>
        /// Add friends model to custom package
        /// </summary>
        /// <param name="PackageId">Package id</param>
        /// <param name="friendsToAdd">Friends model</param>
        /// <returns>Friends model if package found otherwise null</returns>
        public Friends AddFriends(int PackageId, Friends friendsToAdd)
        {
            Friends addedFriends = null;

            try
            {
                using (var context = new CellularContext())
                {
                    Package package = context.PackagesTable.SingleOrDefault((p) => p.PackageId == PackageId);

                    if (package != null)
                    {
                        package.Friends = friendsToAdd;
                        context.SaveChanges();
                        addedFriends = context.PackagesTable.Where((p) => p.PackageId == PackageId).Include(x => x.Friends).Select(s => s.Friends).SingleOrDefault();
                    }
                    return addedFriends;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        /// <summary>
        /// Edit friends for a custom package
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="friendsToEdit"></param>
        /// <returns>Friends model if package found otherwise null</returns>
        public Friends EditFriends(int packageId, Friends friendsToEdit)
        {
            Package foundPackage = null;

            try
            {
                using (var context = new CellularContext())
                {
                    foundPackage = context.PackagesTable.Where((p) => p.PackageId == packageId).Include(x => x.Friends).SingleOrDefault();

                    if (foundPackage == null)
                    {
                        return null;
                    }
                    if (friendsToEdit != null)
                    {
                        friendsToEdit.FriendsId = foundPackage.Friends.FriendsId;
                        context.Entry(foundPackage.Friends).CurrentValues.SetValues(friendsToEdit);
                    }
                    else
                    {
                        foundPackage.Friends.Package = null;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
            if (foundPackage == null)
            {
                throw new IncorrectExeption("Package id");
            }

            return foundPackage.Friends;
        }
    }
}

