using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntityFramework.Enum
{
    public enum Enum_AppointStatus
    {
        /// <summary>
        /// 超过了允许取消的时间
        /// </summary>
        SHOW_NOTCANCEL=-4,
        /// <summary>
        /// 只允许当日预约
        /// </summary>
        SHOW_ONLYTODY=-3,
        /// <summary>
        /// 预约人满了
        /// </summary>
        SHOW_FULLPEOPLE=-2,
        /// <summary>
        /// 不能预约的时间
        /// </summary>
        SHOW_TIMEEXPIRED=-1,
        /// <summary>
        /// 不显示
        /// </summary>
        SHOW_NULL=0,
        /// <summary>
        /// 显示预约
        /// </summary>
        SHOW_APPOINT,

        /// <summary>
        /// 显示取消
        /// </summary>
        SHOW_CANCEL,
        /// <summary>
        /// 显示排队
        /// </summary>
        SHOW_QUEUE,
        /// <summary>
        /// 显示取消排队
        /// </summary>
        SHOW_CANCEL_QUEUE,
    }
}
