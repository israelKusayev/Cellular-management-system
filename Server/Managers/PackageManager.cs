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

        //ctor
        public PackageManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = new LoggerManager(new FileLogger(), "PackageManager.txt");

        }

        /// <summary>
        /// Get package templates
        /// </summary>
        /// <returns>List of Structured packages if succeeded otherwise null</returns>
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

        /// <summary>
        /// Get the package that matches the line
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <returns>package if succeeded otherwise null</returns>
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

        /// <summary>
        /// Add new package to the requested line
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <param name="package">Package details</param>
        /// <returns>Added package if succeeded otherwise null</returns>
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

        /// <summary>
        /// Remove package from the requested line
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <returns>Removed package if succeeded otherwise null</returns>
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

        /// <summary>
        /// Edit an existing package belongs to the requested line 
        /// </summary>
        /// <param name="packageId">Old package</param>
        /// <param name="lineId">Line id</param>
        /// <param name="newPackage">New package details</param>
        /// <returns>Edited package  if succeeded otherwise null</returns>
        public Package EditPackage(int packageId, int lineId, Package newPackage)
        {
            Package package = null;

            try
            {
                if (newPackage.IsPackageTemplate)
                {
                    package = _unitOfWork.Package.Get(newPackage.PackageId);
                    if (package != null)
                    {
                        Line line = _unitOfWork.Line.Get(lineId);
                        package.Lines.Add(line);
                    }
                }
                else
                {
                    package = _unitOfWork.Package.GetPackageWithFriends(packageId);
                    if (package != null)
                    {
                            newPackage.PackageId = package.PackageId;
                            _unitOfWork.Package.Edit(package, newPackage);
                            return _unitOfWork.Package.GetPackageWithFriends(package.PackageId);
                    }
                    _unitOfWork.Complete();

                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
            return package;
        }

        /// <summary>
        /// Remove a specific line from specific package template list
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <returns>True if succeeded otherwise false</returns>
        public bool RemoveLineFromTemplatePackage(int lineId)
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
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
            return false;
        }

        /// <summary>
        /// Add friend's phone numbers to an exsiting package
        /// </summary>
        /// <param name="packageId">Package id</param>
        /// <param name="friendsToAdd">Friends phone numbers</param>
        /// <returns>Added friends if succeeded otherwise null</returns>
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

        /// <summary>
        /// Edit friend's phone numbers inside exsiting package
        /// </summary>
        /// <param name="packageId">Package id</param>
        /// <param name="friendsToEdit">Friends phone numbers</param>
        /// <returns>Edited friends if succeeded otherwise null</returns>
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