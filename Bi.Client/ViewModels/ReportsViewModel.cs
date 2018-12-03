using Bi.Client.Halpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Client.ViewModels
{
    public class ReportsViewModel
    {
        private IFrameNavigationService _frameNavigationService;

        public ReportsViewModel(IFrameNavigationService frameNavigationService)
        {
            _frameNavigationService = frameNavigationService;
        }
    }
}
