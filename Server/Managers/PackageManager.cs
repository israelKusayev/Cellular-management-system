using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Logger;
using Common.Models;
using Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Managers
{
    public class PackageManager
    {
        LoggerManager _logger;

        public PackageManager()
        {
            _logger = new LoggerManager(new FileLogger(), "PackageManager.txt");

        }

        internal List<Package> GetPackageTemplates()
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    List<Package> packages = context.Package.GetPackageTemplate();
                    return packages;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        internal Package GetPackage(int lineId)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Line line = context.Line.GetLineWithPackageAndFriends(lineId);

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

        internal Package AddPackageToLine(int lineId, Package package)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Package addedPackage = null;

                    Line foundLine = context.Line.Get(lineId);

                    if (foundLine != null)
                    {
                        if (package.IsPackageTemplate)
                        {
                            Package existPackage = context.Package.Get(package.PackageId);
                            existPackage.Lines.Add(foundLine);
                            if (existPackage != null)
                            {
                                addedPackage = existPackage;
                            }
                        }
                        else
                        {
                            if (package != null)
                            {
                                foundLine.Package = package;
                                addedPackage = package;
                            }
                        }
                        context.Complete();
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

        internal Package RemovePackageFromLine(int lineId)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Package removedPackage = null;

                    Line line = context.Line.GetLineWithPackageAndFriends(lineId);

                    if (line != null)
                    {
                        removedPackage = line.Package;
                        line.Package = null;
                        context.Complete();
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

        internal Package EditPackage(int packageId, int lineId, Package newPackage)
        {
            Package oldPackage = null;

            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    if (newPackage.IsPackageTemplate)
                    {
                        oldPackage = context.Package.Get(newPackage.PackageId);
                        if (oldPackage != null)
                        {
                            Line line = context.Line.Get(lineId);
                            oldPackage.Lines.Add(line);
                        }
                    }
                    else
                    {
                        oldPackage = context.Package.GetPackageWithFriends(packageId);
                        if (oldPackage != null)
                        {
                            if (oldPackage != null)
                            {
                                newPackage.PackageId = oldPackage.PackageId;
                                context.Package.Edit(oldPackage, newPackage);
                                oldPackage = newPackage;
                            }
                        }
                        context.Complete();

                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            return oldPackage;
        }

        internal void RemoveLineFromTemplatePackage(int lineId)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Line line = context.Line.GetLineWithPackage(lineId);
                    Package package = line.Package;
                    if (package.IsPackageTemplate)
                    {
                        if (line != null)
                        {
                            package.Lines.Remove(line);
                            context.Complete();
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

        internal Friends AddFriends(int packageId, Friends friendsToAdd)
        {
            Friends addedFriends = null;

            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Package package = context.Package.Get(packageId);

                    if (package != null)
                    {
                        package.Friends = friendsToAdd;
                        context.Complete();

                        addedFriends = context.Package.GetPackageWithFriends(packageId).Friends;
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

        internal Friends EditFriends(int packageId, Friends friendsToEdit)
        {
            Package foundPackage = null;

            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    foundPackage = context.Package.GetPackageWithFriends(packageId);

                    if (foundPackage == null)
                    {
                        return null;
                    }
                    if (friendsToEdit != null)
                    {
                        friendsToEdit.FriendsId = foundPackage.Friends.FriendsId;
                        context.Friends.Edit(foundPackage.Friends, friendsToEdit);
                    }
                    else
                    {
                        foundPackage.Friends.Package = null;
                    }
                    context.Complete();
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