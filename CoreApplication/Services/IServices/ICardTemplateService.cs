using CoreEntityFramework.Model;
using CoreEntityFramework.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApplication.Services.IServices
{
    public interface ICardTemplateService:IApplicationService
    {
        Base_PageOutput<View_CardTemplateOutputItem> PageCardTemplate(View_CardTemplateInput input);
        CardTemplate GetCardsTemplateById(int id);
        CardTemplate CreateCardTemplate(CardTemplate model);

        bool UpdateCardTemplate(CardTemplate model);

        List<ViewDoorCardsSelect> GetDoorCards(int doorId);

        List<View_PrevCardTemplateOutput> GetAllDoorCardsTemplate(int doorId);

     

    }
}
