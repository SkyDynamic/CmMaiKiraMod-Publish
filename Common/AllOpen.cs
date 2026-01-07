using System.Linq;
using CardMaker.MAI;
using CardMaker.MAI.MaiStudio;
using HarmonyLib;

namespace CmMaiKiraMod.Common
{
    public class AllOpen
    {
        public static bool Enable = false;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MAISellingCardContext), "set")]
        public static void GetGameSellingCardResponse_deserialize_Postfix(MAISellingCardContext __instance)
        {
            if (!Enable) return;
            
            CmMaiKiraMod.Log.LogInfo($"Origin selling cards: {__instance.GetSellingCardAll().Count}");
            var addedNum = 0;
            foreach (var card in MAIDataManager.GetInstance().CardDataAll)
            {
                var exist = __instance.GetSellingCardAll().Any(item => item.ID == card.Value.name.id);
                if (!exist)
                {
                    var newSellingCard = new MAISellingCard(new GameSellingCard
                    {
                        cardId = card.Value.name.id,
                        startDate = "2019-01-01 00:00:00.0",
                        endDate = "2029-01-01 00:00:00.0",
                        noticeStartDate = "2019-01-01 00:00:00.0",
                        noticeEndDate = "2029-01-01 00:00:00.0"
                    });
                    __instance.GetSellingCardAll().Add(newSellingCard);
                    addedNum++;
                }
            }
            CmMaiKiraMod.Log.LogInfo($"Added {addedNum} cards to selling list");
        }
    }
}