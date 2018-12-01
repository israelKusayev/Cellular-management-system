using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Logger;
using Common.Models;
using Common.RepositoryInterfaces;
using Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Managers
{
    public class PackageManager : IPackageManager
    {
        private IUnitOfWork _unitOfWork;
        LoggerManager _logger;

        public PackageManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = new LoggerManager(new FileLogger(), "PackageManager.txt");

        }

        public List<Package> GetPackageTemplates()
        {
            try
            {
                List<Package> packages = _unitOfWork.Package.GetPackageTemplate();
                return packages;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Package GetPackage(int lineId)
        {
            try
            {
                Line line = _unitOfWork.Line.GetLineWithPackageAndFriends(lineId);

                if (line != null)
                {
                    return line.Package;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Package AddPackageToLine(int lineId, Package package)
        {
            try
            {
                Package addedPackage = null;

                Line foundLine = _unitOfWork.Line.Get(lineId);
                if (foundLine != null)
                {
                    if (package.IsPackageTemplate)
                    {
                        Package existPackage = _unitOfWork.Package.Get(package.PackageId);
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
                    _unitOfWork.Complete();
                }
                return addedPackage;
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
                Package removedPackage = null;

                Line line = _unitOfWork.Line.GetLineWithPackageAndFriends(lineId);

                if (line != null)
                {
                    removedPackage = line.Package;
                    line.Package = null;
                    _unitOfWork.Complete();
                }
                return removedPackage;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Package EditPackage(int packageId, int lineId, Package newPackage)
        {
            Package oldPackage = null;

            try
            {
                if (newPackage.IsPackageTemplate)
                {
                    oldPackage = _unitOfWork.Package.Get(newPackage.PackageId);
                    if (oldPackage != null)
                    {
                        Line line = _unitOfWork.Line.Get(lineId);
                        oldPackage.Lines.Add(line);
                    }
                }
                else
                {
                    oldPackage = _unitOfWork.Package.GetPackageWithFriends(packageId);
                    if (oldPackage != null)
                    {
                        if (oldPackage != null)
                        {
                            newPackage.PackageId = oldPackage.PackageId;
                            _unitOfWork.Package.Edit(oldPackage, newPackage);
                            return _unitOfWork.Package.GetPackageWithFriends(oldPackage.PackageId);
                        }
                    }
                    _unitOfWork.Complete();

                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
            return oldPackage;
        }

        public void RemoveLineFromTemplatePackage(int lineId)
        {
            try
            {
                Line line = _unitOfWork.Line.GetLineWithPackage(lineId);
                Package package = line.Package;
                if (package.IsPackageTemplate)
                {
                    if (line != null)
                    {
                        package.Lines.Remove(line);
                        _unitOfWork.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Friends AddFriends(int packageId, Friends friendsToAdd)
        {
            Friends addedFriends = null;

            try
            {
                Package package = _unitOfWork.Package.Get(packageId);

                if (package != null)
                {
                    package.Friends = friendsToAdd;
                    _unitOfWork.Complete();

                    addedFriends = _unitOfWork.Package.GetPackageWithFriends(packageId).Friends;
                }
                return addedFriends;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Friends EditFriends(int packageId, Friends friendsToEdit)
        {
            Package foundPackage = null;

            try
            {
                foundPackage = _unitOfWork.Package.GetPackageWithFriends(packageId);

                if (foundPackage == null)
                {
                    return null;
                }
                if (friendsToEdit != null)
                {
                    friendsToEdit.FriendsId = foundPackage.Friends.FriendsId;
                    _unitOfWork.Friends.Edit(foundPackage.Friends, friendsToEdit);
                }
                else
                {
                    foundPackage.Friends.Package = null;
                }
                _unitOfWork.Complete();
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