using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [Header("Navigation")]
    public Button nextButton;
    public Button prevButton;
    public TextMeshProUGUI pageIndicator;

    [Header("Content")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image illustrationImage;

    private int currentPage = 0;
    private HowToPlayPage[] pages;

    [System.Serializable]
    private class HowToPlayPage
    {
        public string title;
        [TextArea(5, 10)]
        public string description;
        public Sprite illustration;
    }

    private void Start()
    {
        InitializePages();
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);
        
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousPage);
        
        ShowPage(0);
    }

    private void InitializePages()
    {
        pages = new HowToPlayPage[]
        {
            new HowToPlayPage
            {
                title = "🧬 Welcome to Football DNA Lab!",
                description = "You manage a futuristic laboratory where you create football players through DNA mutation.\n\n" +
                             "Your goal: fulfill orders from football teams and national squads, creating players with the required characteristics."
            },
            new HowToPlayPage
            {
                title = "📋 Orders System",
                description = "Teams send orders with requirements:\n\n" +
                             "• Team/National squad name\n" +
                             "• 1-3 player slots (depends on reputation)\n" +
                             "• Required stats (Speed, Defense, Jumping, etc.)\n" +
                             "• Bonus tags for extra points\n" +
                             "• 30 seconds to accept the order\n\n" +
                             "Complete orders with quality to increase reputation!"
            },
            new HowToPlayPage
            {
                title = "🧪 Mutation Process",
                description = "1. Choose a base subject from 3 available\n" +
                             "2. Select 2-3 DNA strands to merge\n" +
                             "3. Start the mutation process\n" +
                             "4. Wait for completion (there's a chance of failure!)\n" +
                             "5. Send the player to the client\n\n" +
                             "Rating system:\n" +
                             "• 95%+ = Perfect (+30% reward, +8 rep)\n" +
                             "• 85-95% = Excellent (+10% reward, +5 rep)\n" +
                             "• 70-85% = Good (100% reward, +3 rep)\n" +
                             "• 50-70% = Acceptable (70% reward, +1 rep)\n" +
                             "• <50% = Poor (40% reward, -1 rep)"
            },
            new HowToPlayPage
            {
                title = "🧬 DNA Types",
                description = "• 🦁 ANIMAL: Gazelle (speed), Gorilla (strength)\n" +
                             "• ⭐ LEGENDARY: DNA of famous footballers\n" +
                             "• 🌍 ENVIRONMENT: Ice, Lava, Wind\n" +
                             "• 🤖 MECHANICAL: Drone, Magnet, Jetpack\n\n" +
                             "Each DNA has unique traits and effects!"
            },
            new HowToPlayPage
            {
                title = "🔬 Research",
                description = "Unlock new DNA strands through research:\n\n" +
                             "• Choose a category to research\n" +
                             "• Pay money for research\n" +
                             "• Wait for completion\n" +
                             "• Receive a random DNA of the chosen category\n\n" +
                             "Upgrade your lab for faster research!"
            },
            new HowToPlayPage
            {
                title = "⚙️ Laboratory Upgrades",
                description = "Spend money on improvements:\n\n" +
                             "• 🧪 Mutation Chamber - less chance of failure\n" +
                             "• 🔬 Trait Stabilizer - better trait preservation\n" +
                             "• ⚡ Research Speed\n" +
                             "• 📚 DNA Library - more storage space\n" +
                             "• 🚀 Mutation Speed"
            },
            new HowToPlayPage
            {
                title = "📈 Reputation System",
                description = "Each team has its own reputation (0-100):\n\n" +
                             "✅ Successful orders increase reputation\n" +
                             "❌ Failures decrease reputation\n\n" +
                             "Higher reputation unlocks:\n" +
                             "• Better paid orders (up to 2000$)\n" +
                             "• More complex orders (2-3 players)\n" +
                             "• Higher stat requirements (70-85+)\n\n" +
                             "Order difficulty scales with your reputation!"
            },
            new HowToPlayPage
            {
                title = "💡 Tips",
                description = "• Always check order requirements carefully\n" +
                             "• Combine DNA strategically\n" +
                             "• Upgrade lab regularly\n" +
                             "• Monitor team reputations\n" +
                             "• Orders expire after 30 seconds!\n" +
                             "• Game auto-saves every 3 minutes\n\n" +
                             "Good luck creating super footballers! ⚽🧬"
            }
        };
    }

    private void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0) return;
        
        currentPage = Mathf.Clamp(pageIndex, 0, pages.Length - 1);
        
        HowToPlayPage page = pages[currentPage];
        
        if (titleText != null)
            titleText.text = page.title;
        
        if (descriptionText != null)
            descriptionText.text = page.description;
        
        illustrationImage.gameObject.SetActive(true);
        
        if (pageIndicator != null)
            pageIndicator.text = $"{currentPage + 1} / {pages.Length}";
        
        if (prevButton != null)
            prevButton.interactable = currentPage > 0;
        
        if (nextButton != null)
            nextButton.interactable = currentPage < pages.Length - 1;
    }

    public void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    public void PreviousPage()
    {
        ShowPage(currentPage - 1);
    }
}
