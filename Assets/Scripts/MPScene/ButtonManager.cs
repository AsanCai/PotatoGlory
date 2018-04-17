/*
 * 用于管理按钮的冷却效果和启用禁用效果
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsanCai.MPScene {
    public class ButtonManager : MonoBehaviour {
        [HideInInspector]
        public static ButtonManager bm;

        public SkillCooling jumpBtn;
        public SkillCooling layBombsBtn;
        public SkillCooling throwBombsBtn;
        public SkillCooling fireBtn;

        private void Awake() {
            bm = GetComponent<ButtonManager>();
        }

    }
}
